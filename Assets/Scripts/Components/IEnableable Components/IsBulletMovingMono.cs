using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class IsBulletMovingMono : MonoBehaviour
{
    public class Baker : Baker<IsBulletMovingMono>
    {
        public override void Bake(IsBulletMovingMono authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new IsBulletMoving());
        }
    }
}

public struct IsBulletMoving : IComponentData, IEnableableComponent
{

}
