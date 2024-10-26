using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CharacterEntityWithPosition
{
    public Entity entity;
    public Character character;
    public float3 position;
}
