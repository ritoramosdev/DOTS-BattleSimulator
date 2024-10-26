using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

[BurstCompile]
public partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Character>();
    }

    [BurstCompile]
    void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleTon = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        var targetQuery = state.GetEntityQuery(typeof(Character), ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadOnly<IsCharacterAlive>());
        var targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
        var targetTransformArray = targetQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var targetCharacterArray = targetQuery.ToComponentDataArray<Character>(Allocator.TempJob);

        var targetArray = new NativeArray<CharacterEntityWithPosition>(targetEntityArray.Length, Allocator.TempJob);
        var randomNumbers = new NativeArray<int>(targetEntityArray.Length, Allocator.TempJob);

        for (int i = 0; i < targetEntityArray.Length; i++)
        {
            targetArray[i] = new CharacterEntityWithPosition
            {
                entity = targetEntityArray[i],
                position = targetTransformArray[i].Position,
                character = targetCharacterArray[i],
            };

            randomNumbers[i] = UnityEngine.Random.Range(0, randomNumbers.Length);
        }

        targetEntityArray.Dispose();
        targetTransformArray.Dispose();

        new FindTargetJobParallel
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            characterEntitiesWithPosition = targetArray,
        }.Schedule(targetArray.Length, 64).Complete();
    }
}

[BurstCompile]
[WithAll(typeof(Character))]
public partial struct FindTargetJobParallel : IJobParallelFor
{

    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<CharacterEntityWithPosition> characterEntitiesWithPosition;

    [WriteOnly] float3 currentCharacterPosition;
    [WriteOnly] Entity closesTargetEntity;
    [WriteOnly] float3 closestTargetPosition;
    [WriteOnly] CharacterEntityWithPosition characterEntityWithPosition;

    [BurstCompile]
    public void Execute(int index)
    {
        ecb.SetComponent(index, characterEntitiesWithPosition[index].entity, new Target
        {
            targetEntity = Entity.Null,
            targetEntityPosition = float3.zero
        });

        currentCharacterPosition = characterEntitiesWithPosition[index].position;
        closesTargetEntity = Entity.Null;
        closestTargetPosition = float3.zero;

        for (int i = 0; i < characterEntitiesWithPosition.Length; i++)
        {
            characterEntityWithPosition = characterEntitiesWithPosition[i];

            if (characterEntitiesWithPosition[index].entity != characterEntityWithPosition.entity)
            {
                if (closesTargetEntity == Entity.Null)
                {
                    closesTargetEntity = characterEntityWithPosition.entity;
                    closestTargetPosition = characterEntityWithPosition.position;
                }
                else
                {
                    if (math.distance(currentCharacterPosition, characterEntityWithPosition.position) < math.distance(currentCharacterPosition, closestTargetPosition))
                    {
                        closesTargetEntity = characterEntityWithPosition.entity;
                        closestTargetPosition = characterEntityWithPosition.position;
                    }
                }
            }
        }

        if (closesTargetEntity != Entity.Null)
        {
            ecb.SetComponent(index, characterEntitiesWithPosition[index].entity, new Target
            {
                targetEntity = closesTargetEntity,
                targetEntityPosition = closestTargetPosition,
            });

            ecb.SetComponentEnabled<HasTarget>(index, characterEntitiesWithPosition[index].entity, true);
        }
    }
}


[BurstCompile]
[WithAll(typeof(Character))]
[WithDisabled(typeof(InRange))]
public partial struct FindTargetRandomJob : IJobEntity
{

    [ReadOnly] public float deltaTime;
    [ReadOnly] public Unity.Mathematics.Random random;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<CharacterEntityWithPosition> characterEntitiesWithPosition;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<int> randomNumbers;

    [BurstCompile]
    private void Execute([WriteOnly] CharacterAspect character, ref Target target, [EntityIndexInQuery] int sortKey)
    {

        if (target.targetEntity != Entity.Null) return;
        if (character.entity == characterEntitiesWithPosition[randomNumbers[sortKey]].entity) return;

        target.targetEntity = characterEntitiesWithPosition[randomNumbers[sortKey]].entity;
        target.targetEntityPosition = characterEntitiesWithPosition[randomNumbers[sortKey]].position;


        ecb.SetComponentEnabled<HasTarget>(sortKey, character.entity, true);
    }
}