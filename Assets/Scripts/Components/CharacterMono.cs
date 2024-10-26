using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CharacterMono : MonoBehaviour {
    public float moveSpeed;
    public float health;

    public int characterNumber;

    // For Weapon
    public float range;
    public float attackSpeed;

    public class Baker : Baker<CharacterMono>
    {
        public override void Bake(CharacterMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Character
            {
                moveSpeed = authoring.moveSpeed,
                health = authoring.health,
                range = authoring.range,
                attackSpeed = authoring.attackSpeed,
                characterNumber = authoring.characterNumber,
            });

            AddBuffer<BulletDamageBufferElement>(entity);
        }
    }
}

public struct Character : IComponentData {
    public float moveSpeed;
    public float health;

    public int characterNumber;

    // For Weapon
    public float range;
    public float attackSpeed;

}