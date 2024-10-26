using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

public readonly partial struct BulletAspect : IAspect
{
    public readonly Entity entity;
    public readonly RefRW<LocalTransform> _localTransform;
    public readonly RefRW<Bullet> _bullet;

    public readonly float BulletDamage => _bullet.ValueRO.bulletDamage;
    public readonly float BulletSpeed => _bullet.ValueRO.bulletSpeed;

    public float3 TargetPosition
    {
        get => _bullet.ValueRO.targetPosition;
        set => _bullet.ValueRW.targetPosition = value;
    }

    public Entity TargetEntity
    {
        get => _bullet.ValueRO.targetEntity;
        set => _bullet.ValueRW.targetEntity = value;
    }

    public Entity SpawnerEntity
    {
        get => _bullet.ValueRO.spawnerEntity;
        set => _bullet.ValueRW.spawnerEntity = value;
    }
}
