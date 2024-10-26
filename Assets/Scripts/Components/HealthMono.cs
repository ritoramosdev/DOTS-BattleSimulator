using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HealthMono : MonoBehaviour
{
    public class Baker : Baker<HealthMono>
    {
        public override void Bake(HealthMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Health());
        }
    }
}

public struct Health : IComponentData
{
    public float health;
}