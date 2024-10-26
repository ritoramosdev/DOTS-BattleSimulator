using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IsBulletReadyMono : MonoBehaviour
{
    public class Baker : Baker<IsBulletReadyMono>
    {
        public override void Bake(IsBulletReadyMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new IsBulletReady());
        }
    }
}

public struct IsBulletReady : IComponentData, IEnableableComponent
{

}
