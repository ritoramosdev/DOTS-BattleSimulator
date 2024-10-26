using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TargetMono : MonoBehaviour
{
    public class Baker : Baker<AttackCooldownMono>
    {
        public override void Bake(AttackCooldownMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Target());
        }
    }
}

public struct Target : IComponentData
{
    public Entity targetEntity;
    public float3 targetEntityPosition;
}