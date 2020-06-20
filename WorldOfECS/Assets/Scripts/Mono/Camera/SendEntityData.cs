using UniRx;
using UnityEngine;

//TODO remove for messagebroker

namespace WorldOfECS.Event
{
//used for sending the required data for raycasting. basic mono
    [RequireComponent(typeof(Camera))]
    public class SendEntityData : MonoBehaviour
    {
        private void Start()
        {
            MessageBroker.Default.Publish(GetComponent<Camera>());
        }
    }
}