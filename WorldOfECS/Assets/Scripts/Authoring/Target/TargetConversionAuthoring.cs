using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using WorldOfECS.Data;
using WorldOfECS.Event;

namespace WorldOfECS.Animation
{
    public class TargetConversionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite<SpherecastData>(),
                ComponentType.ReadWrite<SpherePhysicsCommandData>(),
                ComponentType.ReadOnly<CopyTransformFromGameObject>()));


            //*NOTE We are batching raycast collision for job scheduling (good for alot of enemy sphere cast until change to Job raycast system).
            //Set the Enemy sphere cast radius 
            dstManager.SetComponentData(entity, new SpherePhysicsCommandData()
            {
                distance =  5,
                origin = transform.position,
                direction = Vector3.forward,
                layerMask = 1 << 8,
                radius = 4,
                minimumCommandPerJob = 1
            });
            
        }
    }
}
