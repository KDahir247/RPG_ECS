using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using WorldOfECS.Data;
using WorldOfECS.Foundation;

namespace WorldOfECS.Authoring
{
    [RequiresEntityConversion]
    public class PlayerConversionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {

                dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite <RaycastData> (), 
                ComponentType.ReadOnly<RayPhysicsCommandData>(),
                ComponentType.ReadOnly<CopyTransformFromGameObject>()
                ));

            //Set the Player Raycast Physics property
            dstManager.SetComponentData(entity, new RayPhysicsCommandData()
            {
                distance = 25f,
                maxHits = 1,
                minimumCommandPerJob = 1,
                layerMask = 1 << 8 // layermask 8: raycastable (change to -5 for all)
            });
            
        }
    }
}