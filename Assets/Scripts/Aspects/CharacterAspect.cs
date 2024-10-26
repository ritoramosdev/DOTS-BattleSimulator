using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

public readonly partial struct CharacterAspect : IAspect
{
    public readonly Entity entity;
    public readonly RefRW<LocalTransform> _localTransform;
    public readonly RefRW<Character> _character;

    public readonly float MoveSpeed => _character.ValueRO.moveSpeed;
    public readonly float Health => _character.ValueRO.health;
    public readonly float Range => _character.ValueRO.range;
    public readonly float AttackSpeed => _character.ValueRO.attackSpeed;

    private readonly DynamicBuffer<BulletDamageBufferElement> _bulletDamageBuffer;

    public void DamageCharacter() {
        foreach (var bulletDamage in _bulletDamageBuffer) {
            _character.ValueRW.health -= bulletDamage.damage;
        }

        _bulletDamageBuffer.Clear();
    }
}
