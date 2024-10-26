using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TextCore.Text;

[BurstCompile]
[UpdateAfter(typeof(BulletMoveToTargetSystem))]
public partial struct AttackCooldownSystem : ISystem
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

        new AttackCooldownJob
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
[WithAll(typeof(Character))]
public partial struct AttackCooldownJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    private void Execute(ref Character character, ref AttackCooldown attackCooldown, [EntityIndexInQuery] int sortKey) {
        attackCooldown.attackCooldown -= deltaTime;
    }
}
