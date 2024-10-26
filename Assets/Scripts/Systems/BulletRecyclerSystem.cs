using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(BulletMoveToTargetSystem))]
public partial struct BulletRecyclerSystem : ISystem
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
        var ecbSingleTon = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new BulletRecyclerJob
        {
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
[WithAll(typeof(Bullet))]
[WithDisabled(typeof(IsBulletReady))]
[WithDisabled(typeof(IsBulletMoving))]
[WithAll(typeof(IsBulletDead))]
public partial struct BulletRecyclerJob : IJobEntity
{
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    private void Execute([WriteOnly] BulletAspect bullet, [EntityIndexInQuery] int sortKey) {
        ecb.SetComponentEnabled<IsBulletDead>(sortKey, bullet.entity, false);
        ecb.SetComponentEnabled<IsBulletReady>(sortKey, bullet.entity, true);
        //ecb.DestroyEntity(sortKey, bullet.entity);
    }
}