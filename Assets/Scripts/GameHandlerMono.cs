using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class GameHandlerMono : MonoBehaviour {
    public float2 dimensions;
    public int characterAmountToSpawn;
    public int bulletAmountToSpawn;
    public GameObject characterPrefab;
    public GameObject bulletPrefab;


    public class Baker : Baker<GameHandlerMono>
    {
        public override void Bake(GameHandlerMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameHandler
            {
                dimensions = authoring.dimensions,
                characterAmountToSpawn = authoring.characterAmountToSpawn,
                bulletAmountToSpawn = authoring.bulletAmountToSpawn,
                characterPrefab = GetEntity(authoring.characterPrefab, TransformUsageFlags.Dynamic),
                bulletPrefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct GameHandler : IComponentData {
    public float2 dimensions;
    public int characterAmountToSpawn;
    public int bulletAmountToSpawn;

    public Entity characterPrefab;
    public Entity bulletPrefab;
}
