using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletMono : MonoBehaviour
{
    public float bulletDamage;
    public float bulletSpeed;

    public class Baker : Baker<BulletMono>
    {
        public override void Bake(BulletMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Bullet
            {
                bulletDamage = authoring.bulletDamage,
                bulletSpeed = authoring.bulletSpeed,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float bulletDamage;
    public float bulletSpeed;

    public float3 targetPosition;
    public Entity targetEntity;
    public Entity spawnerEntity;
}
