using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

public readonly partial struct GameHandlerAspect : IAspect
{
    public readonly Entity entity;
    private readonly RefRO<GameHandler> _gameHandler;

    public float2 Dimensions => _gameHandler.ValueRO.dimensions;
    public int CharacterAmountToSpawn => _gameHandler.ValueRO.characterAmountToSpawn;
    public int BulletAmountToSpawn => _gameHandler.ValueRO.bulletAmountToSpawn;
    public Entity CharacterPrefab => _gameHandler.ValueRO.characterPrefab;
    public Entity BulletPrefab => _gameHandler.ValueRO.bulletPrefab;

    public LocalTransform GetRandomTransform() {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

    private float3 GetRandomPosition() {
        float3 randomPosition;

        randomPosition = new float3 {
            x = UnityEngine.Random.Range(-_gameHandler.ValueRO.dimensions.x, _gameHandler.ValueRO.dimensions.x),
            y = 1f,
            z = UnityEngine.Random.Range(-_gameHandler.ValueRO.dimensions.y, _gameHandler.ValueRO.dimensions.y)
        };

        return randomPosition;
    }
}
