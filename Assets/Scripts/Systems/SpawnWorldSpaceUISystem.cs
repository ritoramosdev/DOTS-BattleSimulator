using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class SpawnWorldSpaceUISystem : SystemBase
{
    //public event EventHandler OnCharacterSpawn;
    public Action<int, float3, Entity> OnCharacterSpawn;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var targetQuery = GetEntityQuery(typeof(Character), ComponentType.ReadOnly<LocalTransform>());
        var targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
        var targetCharacterArray = targetQuery.ToComponentDataArray<Character>(Allocator.TempJob);
        var targetLocalTransformArray = targetQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

        for (int i = 0; i < targetEntityArray.Length; i++)
        {
            OnCharacterSpawn?.Invoke(targetCharacterArray[i].characterNumber, targetLocalTransformArray[i].Position, targetEntityArray[i]);
        }
        
        Debug.Log(targetEntityArray.Length);
        if (targetEntityArray.Length > 0)
            Enabled = false;
    }
}
