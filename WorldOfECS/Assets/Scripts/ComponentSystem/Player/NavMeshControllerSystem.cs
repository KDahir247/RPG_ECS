using Unity.Entities;
using Unity.Jobs;
using UnityEngine.AI;
using WOrldOfECS.Data;

namespace WOrldOfECS.ComponentSystem
{
    public class NavMeshControllerSystem : JobComponentSystem
    {
        readonly NavMeshPath _path = new NavMeshPath();

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            Entities
                .WithoutBurst()
                .ForEach((Entity entity,
                    NavMeshAgent navAgent,
                    ref NavAgentData agentData) =>
                {
                    navAgent.CalculatePath(agentData.destination, _path);
                    if (_path.status != NavMeshPathStatus.PathPartial && _path.status != NavMeshPathStatus.PathInvalid)
                    {
                        navAgent.SetDestination(agentData.destination);
                    }
                }).Run();
            
            return inputDeps;
        }
    }
}