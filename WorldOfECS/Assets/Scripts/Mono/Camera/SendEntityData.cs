using UnityEngine;
using WorldOfECS.Event;

//used for sending the required data for raycasting. basic mono
[RequireComponent(typeof(Camera))]
public class SendEntityData : MonoBehaviour
{
    private readonly BehaviourEventBus _bus = new BehaviourEventBus();

    private void Start()
    {
        _bus.AddToBus(GetComponent<Camera>());
    }

    private void OnDisable()
    {
        if (!BehaviourEventBus.IsClear)
            _bus.ClearEventBusCache();
    }
}