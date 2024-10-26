using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InRangeMono : MonoBehaviour
{
    public class Baker : Baker<InRangeMono>
    {
        public override void Bake(InRangeMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new InRange());
        }
    }
}

public struct InRange : IComponentData, IEnableableComponent
{

}
