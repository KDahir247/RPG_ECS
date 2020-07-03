using System;
using Unity.Entities;
using Unity.Kinematica;
using Unity.Transforms;
using UnityEngine;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.Data;

namespace WorldOfECS.Authoring
{
    //TODO add Scriptable Object to customize the DistanceInputCommandData since there will be a large amount of enemies
    [RequiresEntityConversion]
    public class PlayerConversionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        //TEST TODO WORKS
        private Entity _entity;
        private EntityManager _manager;
        
        private void InitializeDataStack(in Entity entity, ref EntityManager dstManager)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite<PhysicsCastData>(),
                ComponentType.ReadOnly<RayPhysicsCommandData>(),
                ComponentType.ReadOnly<CopyTransformFromGameObject>(),
                ComponentType.ReadWrite<MovementData>(),
                ComponentType.ReadWrite<CombatData>()
            ));

            //Set the Player Raycast Physics property
            dstManager.SetComponentData(entity, new RayPhysicsCommandData
            {
                distance = 25f,
                maxHits = 1,
                minimumCommandPerJob = 5,
                layerMask = 0x100
            });

            //we reach the target in the start of the frame since the target is the origin of the player.
            //until player press somewhere
            dstManager.SetComponentData(entity, new PhysicsCastData
            {
                hasReached = true,
                hasTarget = false
            });
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            InitializeDataStack(entity, ref dstManager);
            
            
            //testing TODO WORKS
            _entity = entity;
            _manager = dstManager;
        }

        //Play Idle Animation
        private void Awake()
        {
            ref var initMotionSynthesizer = ref gameObject.GetComponent<Kinematica>().Synthesizer.Ref;

            initMotionSynthesizer
                .Action()
                .Push(initMotionSynthesizer.Query.Where(Locomotion.Default)
                    .And(Idle.Default));
        }

        private void LateUpdate()
        {
            //TODO WORKS
            Debug.Log(_manager.GetComponentObject<Transform>(_entity).gameObject.name);

        }
    }
}