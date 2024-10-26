using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class SpawnEntitySystem : SystemBase
{
    public int amountToSpawn = 10;

    [BurstCompile]
    protected override void OnCreate()
    {
        Enabled = false;
        RequireForUpdate<GameHandler>();
    }

    [BurstCompile]
    protected override void OnDestroy() { }

    [BurstCompile]
    protected override void OnUpdate()
    {
        Enabled = false;

        var gameHandlerEntity = SystemAPI.GetSingletonEntity<GameHandler>();
        var gameHandler = SystemAPI.GetAspect<GameHandlerAspect>(gameHandlerEntity);
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        for (int i = 0; i < this.amountToSpawn; i++)
        {
            // Character
            var newCharacter = ecb.Instantiate(gameHandler.CharacterPrefab);
            var newCharacterTransform = gameHandler.GetRandomTransform();

            ecb.SetComponent(newCharacter, new Character
            {
                health = UnityEngine.Random.Range(5f, 10f),
                range = UnityEngine.Random.Range(5f,10f),
                attackSpeed = UnityEngine.Random.Range(0.5f,2.5f),
                moveSpeed = UnityEngine.Random.Range(3f, 7f),
                characterNumber = i + 1,
            });

            ecb.SetComponent(newCharacter, new LocalTransform
            {
                Position = newCharacterTransform.Position,
                Rotation = newCharacterTransform.Rotation,
                Scale = newCharacterTransform.Scale
            });

            ecb.SetComponentEnabled<HasTarget>(newCharacter, false);
            ecb.SetComponentEnabled<InRange>(newCharacter, false);
            ecb.SetComponentEnabled<IsCharacterAlive>(newCharacter, true);
        }

        for (int i = 0; i < gameHandler.BulletAmountToSpawn; i++)
        {

            // Bullet
            var newBullet = ecb.Instantiate(gameHandler.BulletPrefab);
            var newBulletTransform = gameHandler.GetRandomTransform();

            ecb.SetComponent(newBullet, new LocalTransform
            {
                Position = newBulletTransform.Position,
                Rotation = newBulletTransform.Rotation,
                Scale = 0f,
            });

            ecb.SetComponent(newBullet, new Bullet
            {
                bulletDamage = 1f,
                bulletSpeed = 5f,
                targetPosition = float3.zero,
            });

            ecb.SetComponentEnabled<IsBulletReady>(newBullet, true);
            ecb.SetComponentEnabled<IsBulletMoving>(newBullet, false);
            ecb.SetComponentEnabled<IsBulletDead>(newBullet, false);
        }

        ecb.Playback(EntityManager);
    }
}
