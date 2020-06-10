using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using MeshCollider = Unity.Physics.MeshCollider;

public class Mover : MonoBehaviour
{
    private Mouse s = Mouse.current;
    [SerializeField]
    private NavMeshAgent agent;

    private Ray lastRay;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObjectEntity e = gameObject.AddComponent<GameObjectEntity>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (s.leftButton.isPressed)
        {
            MoveToCursor();
        }
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(s.position.ReadValue());
        
        Debug.Log(ray.origin);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            agent.SetDestination(hit.point);
        }
    }
}
