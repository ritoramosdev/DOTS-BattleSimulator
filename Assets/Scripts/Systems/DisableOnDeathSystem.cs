using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct DisableOnDeathSystem : ISystem
{
    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Character>();
    }

    [BurstCompile]
    void OnDestroy(ref SystemState state) { }

    

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleTon = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new DisableOnDeathJob
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct DisableOnDeathJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;

    [BurstCompile]
    private void Execute(CharacterAspect character, [EntityIndexInQuery] int sortKey)
    {
        if (character.Health <= 0) {

            ecb.SetComponent(sortKey, character.entity, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 0f,
            });

            ecb.SetComponent(sortKey, character.entity, new Target
            {
                targetEntity = Entity.Null,
                targetEntityPosition = float3.zero
            });

            ecb.SetComponentEnabled<IsCharacterAlive>(sortKey, character.entity, false);
        }
    }
}