using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IsCharacterAliveMono : MonoBehaviour
{
    public class Baker : Baker<IsCharacterAliveMono>
    {
        public override void Bake(IsCharacterAliveMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new IsCharacterAlive());
        }
    }
}

public struct IsCharacterAlive : IComponentData, IEnableableComponent
{

}
