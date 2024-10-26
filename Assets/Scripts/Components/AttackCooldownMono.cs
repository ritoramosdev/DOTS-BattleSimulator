using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AttackCooldownMono : MonoBehaviour
{
    public float attackCooldown;

    public class Baker : Baker<AttackCooldownMono>
    {
        public override void Bake(AttackCooldownMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new AttackCooldown
            {
                attackCooldown = authoring.attackCooldown,
            });
        }
    }
}

public struct AttackCooldown : IComponentData
{
    public float attackCooldown;
}
