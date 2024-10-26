using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.TextCore.Text;

[BurstCompile]
public partial class MovePlayerNumberUISystem : SystemBase
{
    //public event EventHandler OnCharacterSpawn;
    //public Action OnCharacterMove;

    [BurstCompile]
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        //OnCharacterMove?.Invoke();
    }
}
