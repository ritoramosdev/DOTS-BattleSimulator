using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerNumberUI : MonoBehaviour
{
    public Entity characterEntity;

    private void Update()
    {
        MovePlayerNumberUI();
    }

    public void MovePlayerNumberUI() {
        var localTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(characterEntity);
        var character = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Character>(characterEntity);

        transform.position = new Vector3(localTransform.Position.x, localTransform.Position.y + 1.5f, localTransform.Position.z);

        if (character.health <= 0)
            gameObject.SetActive(false);
    }
}
