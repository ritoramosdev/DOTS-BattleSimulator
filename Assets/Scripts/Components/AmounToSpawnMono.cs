using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class AmountToSpawnMono : MonoBehaviour
{
    public class Baker : Baker<AmountToSpawnMono>
    {
        public override void Bake(AmountToSpawnMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new AmountToSpawn
            {
                amountToSpawn = 10
            });
        }
    }
}

public struct AmountToSpawn : IComponentData
{
    public float amountToSpawn;
}