using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IsBulletDeadMono : MonoBehaviour
{
    public class Baker : Baker<IsBulletDeadMono>
    {
        public override void Bake(IsBulletDeadMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new IsBulletDead());
        }
    }
}

public struct IsBulletDead : IComponentData, IEnableableComponent
{

}
