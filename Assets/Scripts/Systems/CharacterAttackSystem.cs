using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

[BurstCompile]
[UpdateAfter(typeof(MoveToTargetSystem))]
public partial struct CharacterAttackSystem : ISystem
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
        var ecbSingleTon = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var gameHandler = SystemAPI.GetSingleton<GameHandler>();

        var targetBulletQuery = state.GetEntityQuery(typeof(Bullet), ComponentType.ReadOnly<IsBulletReady>());
        var targetBulletEntityArray = targetBulletQuery.ToEntityArray(Allocator.TempJob);

        var targetBulletArray = new NativeArray<Entity>(targetBulletEntityArray.Length, Allocator.TempJob);


        var targetCharacterQuery = state.GetEntityQuery(typeof(Character), ComponentType.ReadOnly<LocalTransform>(), 
                                         ComponentType.ReadOnly<AttackCooldown>(), ComponentType.ReadOnly<HasTarget>(),
                                         ComponentType.ReadOnly<InRange>(), ComponentType.ReadOnly<Target>());

        var targetCharacterEntityArray = targetCharacterQuery.ToEntityArray(Allocator.TempJob);
        var targetCharacterTransformArray = targetCharacterQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
        var targetCharactersArray = targetCharacterQuery.ToComponentDataArray<Character>(Allocator.TempJob);
        var targetCharacterAttackCooldownArray = targetCharacterQuery.ToComponentDataArray<AttackCooldown>(Allocator.TempJob);
        var targetCharacterTargetsArray = targetCharacterQuery.ToComponentDataArray<Target>(Allocator.TempJob);

        var doesCharacterTargetEntityExist = state.EntityManager.UniversalQuery.GetEntityQueryMask();

        new CharacterAttackJobParallel
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            gameHandler = gameHandler,
            characterEntities = targetCharacterEntityArray,
            characters = targetCharactersArray,
            characterTransforms = targetCharacterTransformArray,
            bulletEntities = targetBulletEntityArray,
            characterAttackCooldowns = targetCharacterAttackCooldownArray,
            characterTargets = targetCharacterTargetsArray,
            doesCharacterTargetEntityExist = doesCharacterTargetEntityExist,
        }.Schedule(targetCharacterEntityArray.Length, 64).Complete();
    }
}

[BurstCompile]
[WithAll(typeof(Character))]
[WithAll(typeof(IsCharacterAlive))]
public partial struct CharacterAttackJobParallel : IJobParallelFor
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [ReadOnly] public GameHandler gameHandler;
    [WriteOnly] public Entity spawnedBullet;
    [ReadOnly] public EntityQueryMask doesCharacterTargetEntityExist;


    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> characterEntities;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<Character> characters;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<LocalTransform> characterTransforms;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<AttackCooldown> characterAttackCooldowns;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<Target> characterTargets;
    [NativeDisableParallelForRestriction][ReadOnly][DeallocateOnJobCompletion] public NativeArray<Entity> bulletEntities;

    [BurstCompile]
    public void Execute(int index)
    {
        if (characterTargets[index].targetEntity != Entity.Null)
        {

            if (characterAttackCooldowns[index].attackCooldown > 0) return;

            ecb.SetComponent(index, bulletEntities[index], new LocalTransform
            {
                Position = characterTransforms[index].Position,
                Rotation = quaternion.identity,
                Scale = 0.25f,
            });

            ecb.SetComponent(index, bulletEntities[index], new Bullet
            {
                bulletDamage = 1f,
                bulletSpeed = 5f,
                targetPosition = characterTargets[index].targetEntityPosition,
                targetEntity = characterTargets[index].targetEntity,
                spawnerEntity = characterEntities[index],
            });

            ecb.SetComponentEnabled<IsBulletReady>(index, bulletEntities[index], false);
            ecb.SetComponentEnabled<IsBulletMoving>(index, bulletEntities[index], true);

            ecb.SetComponent(index, characterEntities[index], new AttackCooldown
            {
                attackCooldown = 1 / characters[index].attackSpeed,
            });
        }
        else
        {
            ecb.SetComponent(index, characterEntities[index], new Target
            {
                targetEntity = Entity.Null,
                targetEntityPosition = float3.zero,
            });

            ecb.SetComponentEnabled<HasTarget>(index, characterEntities[index], false);
            ecb.SetComponentEnabled<InRange>(index, characterEntities[index], false);

            //Debug.LogError($"Entity: { characterEntities[index] }, Target is Dead!");
        }
    }
}