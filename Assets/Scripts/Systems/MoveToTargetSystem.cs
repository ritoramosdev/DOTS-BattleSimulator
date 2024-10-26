using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

[BurstCompile]
[UpdateAfter(typeof(RotateToTargetSystem))]
public partial struct MoveToTargetSystem : ISystem
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

        new MoveToTargetJob
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
[WithAll(typeof(Character))]
[WithAll(typeof(HasTarget))]
public partial struct MoveToTargetJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [WriteOnly] float3 targetDirection;
    [WriteOnly] float distanceToTarget;

    [BurstCompile]
    private void Execute(CharacterAspect character, ref Target target, [EntityIndexInQuery] int sortKey) {
        targetDirection = target.targetEntityPosition - character._localTransform.ValueRO.Position;
        distanceToTarget = math.distance(target.targetEntityPosition, character._localTransform.ValueRO.Position);

        if (distanceToTarget < character.Range)
            ecb.SetComponentEnabled<InRange>(sortKey, character.entity, true);
        else
        {
            character._localTransform.ValueRW.Position += math.normalize(targetDirection) * character.MoveSpeed * deltaTime;
            ecb.SetComponentEnabled<InRange>(sortKey, character.entity, false);
        }
    }
}