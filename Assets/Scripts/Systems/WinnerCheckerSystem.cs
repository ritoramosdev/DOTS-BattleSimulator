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
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class WinnerCheckerSystem : SystemBase
{
    public Action<Entity> OnWinnerDecided;
    Entity winningEntity;

    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        var targetQuery = GetEntityQuery(typeof(Character), ComponentType.ReadOnly<LocalTransform>());
        var targetEntityArray = targetQuery.ToEntityArray(Allocator.TempJob);
        var winnerCounter = 0;

        for (var i = 0; i < targetEntityArray.Length; i++) {

            if(EntityManager.IsComponentEnabled<IsCharacterAlive>(targetEntityArray[i])){
                winningEntity = targetEntityArray[i];
                winnerCounter++;
            }
        }

        if (winnerCounter == 1) {

            OnWinnerDecided?.Invoke(winningEntity);
            Enabled = false;

            Debug.LogError($"SIMULATION DONE! WINNDER IS {winningEntity}");
        }
    }
}
