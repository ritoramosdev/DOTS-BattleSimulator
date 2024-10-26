using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HasTargetMono : MonoBehaviour
{
    public class Baker : Baker<HasTargetMono>
    {
        public override void Bake(HasTargetMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HasTarget());
        }
    }
}
public struct HasTarget : IComponentData, IEnableableComponent { 

}