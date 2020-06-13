using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using WorldOfECS.Data;


namespace WorldOfECS.Authoring
{
    [RequiresEntityConversion]
    public class PlayerConversionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite <NavAgentData> (),
            ComponentType.ReadOnly<PhysicsCommandData>()
                ));

            //Set the Player Raycast Physics property
            dstManager.SetComponentData(entity, new PhysicsCommandData()
            {
                distance = 300.00f,
                maxHits = 1,
                minimumCommandPerJob = 1,
                layerMask = 1 << 8 // layermask 8: raycastable (change to -5 for all)
            });

            //Copy the Injected GameObject local to world matrix and pass it to the entity
            dstManager.AddComponentData(entity, new CopyTransformFromGameObject());

        }
    }
}