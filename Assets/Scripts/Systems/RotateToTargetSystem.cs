using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(FindTargetSystem))]
public partial struct RotateToTargetSystem : ISystem
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

        new RotateToTargetJob
        {
            deltaTime = deltaTime,
            ecb = ecbSingleTon.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        }.ScheduleParallel();
    }
}

[BurstCompile]
[WithAll(typeof(Character))]
[WithAll(typeof(HasTarget))]
public partial struct RotateToTargetJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [WriteOnly] public EntityCommandBuffer.ParallelWriter ecb;
    [WriteOnly] float3 targetDirection;
    [WriteOnly] quaternion lookRotation;

    [BurstCompile]
    private void Execute(CharacterAspect character, in Target target, [EntityIndexInQuery] int sortKey)
    {
        targetDirection = target.targetEntityPosition - character._localTransform.ValueRO.Position;
        lookRotation = quaternion.LookRotation(math.normalize(targetDirection), character._localTransform.ValueRO.Up());
        character._localTransform.ValueRW.Rotation = lookRotation;
    }
}