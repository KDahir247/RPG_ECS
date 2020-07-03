using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using WorldOfECS.Data;

namespace WorldOfECS.ComponentSystem
{
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, FloatMode = FloatMode.Fast, CompileSynchronously = false)]
    public class DistanceJobSystem : JobComponentSystem
    {
        private PhysicsWorld _physicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnStartRunning()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            _physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            _stepPhysicsWorld.FinalJobHandle.Complete();

            var distanceHit = new NativeList<DistanceHit>(1, Allocator.TempJob);

            Entities.WithoutBurst()
                .ForEach((Entity entity, ref PhysicsCastData physicsCastData, in Translation position,
                    in DistanceInputCommandData distanceInputCommandData) =>
                {
                    var pointDistanceInput = new PointDistanceInput
                    {
                        Position = position.Value,
                        MaxDistance = distanceInputCommandData.maxDistance,
                        Filter = distanceInputCommandData.collision
                    };

                    var pointDistanceJobHandle = new PointDistanceJob
                    {
                        pointDistanceInput = pointDistanceInput,
                        physicsWorld = _physicsWorld,
                        distanceHits = distanceHit
                    }.Schedule();

                    pointDistanceJobHandle.Complete();

                    if (distanceHit.Length > 0)
                    {
                        physicsCastData.entity = distanceHit[0].Entity;
                        physicsCastData.hasTarget = true;
                        physicsCastData.status = Status.Targetable;
                    }
                    else
                    {
                        physicsCastData.hasTarget = false;
                        physicsCastData.status = Status.Idle;
                    }
                })
                .Run();

            //DistanceInputCommandData is needed so the job system can distinguish which entity should schedule this job on
            //any entity with DistanceInputCommandData (mostly Enemies)
            var jobHandle = Entities.WithAll<DistanceInputCommandData>()
                .ForEach((Entity entity, ref PhysicsCastData physicsCastData, in Translation position) =>
                {
                    if (!physicsCastData.hasTarget) return;

                    //the stopping distance;
                    physicsCastData.stoppingDistance = 1.5f;
                    
                    physicsCastData.hit.point = distanceHit[0].Position;
                    physicsCastData.hit.distance = distanceHit[0].Distance;

                    if (math.distance(physicsCastData.hit.point, position.Value) >= physicsCastData.stoppingDistance)
                        physicsCastData.hasReached = false;
                })
                .WithReadOnly(distanceHit)
                .Schedule(inputDeps);

            jobHandle.Complete();

            distanceHit.Dispose(jobHandle);

            return jobHandle;
        }

        private struct PointDistanceJob : IJob
        {
            [ReadOnly] public PointDistanceInput pointDistanceInput;
            public NativeList<DistanceHit> distanceHits;
            [ReadOnly] public PhysicsWorld physicsWorld;

            public void Execute()
            {
                physicsWorld.CalculateDistance(pointDistanceInput, ref distanceHits);
            }
        }
    }
}