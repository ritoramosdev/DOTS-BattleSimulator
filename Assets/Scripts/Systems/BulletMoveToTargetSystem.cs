using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(CharacterAttackSystem))]
public partial struct BulletMoveToTargetSystem : ISystem
{
    [BurstCompile]
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Bullet>();
    }


    [BurstCompile]
    void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleTon = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var doesEntityExist = state.EntityManager.UniversalQuery.GetEntityQueryMask();

        new BulletMoveToTargetJob
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            doesEntityExist = doesEntityExist,
            isCharacterAlive = state.GetComponentLookup<IsCharacterAlive>(true),
            
        }.ScheduleParallel();

    }
}

[BurstCompile]
[WithAll(typeof(Bullet))]
[WithAll(typeof(IsBulletMoving))]
public partial struct BulletMoveToTargetJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [WriteOnly] float3 targetDirection;
    [WriteOnly] public float3 targetPosition;
    [WriteOnly] float distanceToTarget;

    [ReadOnly] public EntityQueryMask doesEntityExist;

    [ReadOnly] public ComponentLookup<IsCharacterAlive> isCharacterAlive;

    [BurstCompile]
    private void Execute([WriteOnly] BulletAspect bullet, [EntityIndexInQuery] int sortKey)
    {

        if (!isCharacterAlive.IsComponentEnabled(bullet.SpawnerEntity) || !isCharacterAlive.IsComponentEnabled(bullet.TargetEntity))
        {
            ecb.SetComponentEnabled<IsBulletMoving>(sortKey, bullet.entity, false);
            ecb.SetComponentEnabled<IsBulletDead>(sortKey, bullet.entity, true);

            bullet._localTransform.ValueRW.Scale = 0f;

            return;
        }

        // Move Bullet
        targetDirection = bullet.TargetPosition - bullet._localTransform.ValueRO.Position;
        bullet._localTransform.ValueRW.Position += math.normalize(targetDirection) * bullet.BulletSpeed * deltaTime;

        distanceToTarget = math.distance(bullet.TargetPosition, bullet._localTransform.ValueRO.Position);
        if (distanceToTarget < 0.25f)
        {
            ecb.SetComponentEnabled<IsBulletMoving>(sortKey, bullet.entity, false);
            ecb.SetComponentEnabled<IsBulletDead>(sortKey, bullet.entity, true);

            bullet._localTransform.ValueRW.Scale = 0f;

            var bulletDamageBuffer = new BulletDamageBufferElement { damage = bullet.BulletDamage };
            ecb.AppendToBuffer(sortKey, bullet.TargetEntity, bulletDamageBuffer);
        }
    }
}
