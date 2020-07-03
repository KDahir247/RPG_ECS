using Unity.Entities;
using Unity.Kinematica;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using WorldOfECS.Animation.Annotation;
using WorldOfECS.Data;

namespace WorldOfECS.Animation
{
    //TODO add Scriptable Object to customize the DistanceInputCommandData since there will be a large amount of enemies
    [RequiresEntityConversion]
    public class TargetConversionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            InitializeDataStack(entity, ref dstManager);
        }

        private void InitializeDataStack(in Entity entity, ref EntityManager dstManager)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                ComponentType.ReadWrite<PhysicsCastData>(),
                ComponentType.ReadOnly<DistanceInputCommandData>(),
                ComponentType.ReadOnly<CopyTransformFromGameObject>(),
                ComponentType.ReadWrite<MovementData>(),
                ComponentType.ReadWrite<CombatData>()));


            //*NOTE We are batching raycast collision for job scheduling (good for alot of enemy sphere cast until change to Job raycast system).
            //Set the Enemy sphere cast radius 
            dstManager.SetComponentData(entity, new DistanceInputCommandData
            {
                maxDistance = 10,
                collision = new CollisionFilter
                {
                    BelongsTo = 0xffffffff,
                    CollidesWith = 0x2
                }
            });


            //we reach the target in the start of the frame since the target is the origin of the enemy.
            //until enemy detect the player
            dstManager.SetComponentData(entity, new PhysicsCastData
            {
                hasReached = true,
                hasTarget = false
            });
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
    }
}