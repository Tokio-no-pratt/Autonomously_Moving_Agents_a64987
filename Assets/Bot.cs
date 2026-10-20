using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;

   
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Seek (Vector3 location)
    {
        agent.SetDestination(location);
    }
    
    void Update()
    {
        Seek(target.transform.position);
    }
}
