
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;
    Drive ds;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ds = target.GetComponent<Drive>();
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - transform.position;
        agent.SetDestination(transform.position - fleeVector);
    }

    void Pursue()
    {
        Vector3 targetDir = target.transform.position - transform.position;

        float relativeHeading = Vector3.Angle(
            transform.forward,
            target.transform.forward
        );

        float toTarget = Vector3.Angle(
            transform.forward,
            targetDir
        );

        if ((toTarget > 90 && relativeHeading < 20) || ds.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }

        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed);

        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    void Evade()
    {
        Vector3 targetDir = target.transform.position - transform.position;

        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed);

        Flee(target.transform.position + target.transform.forward * lookAhead);
    }
    Vector3 wanderTarget = Vector3.zero;
    void wander()
    {
        float wanderRadius = 10;
        float wanderDistance = 20;
        float wanderJitter = 1;

        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        Seek(targetWorld);

    }

    void Hide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        for(int i = 0; i < world.Instance.GetHidingSpots().Length; i++)
        {
            Vector3 hiderDir = world.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePos = world.Instance.GetHidingSpots()[i].transform.position + hiderDir.normalized * 10;

            if (Vector3.Distance(this.transform.position, hidePos)< dist)
            {
                chosenSpot = hidePos;
                dist = Vector3.Distance(this.transform.position, hidePos);
            }

        }


        Seek(chosenSpot);
    }
    void Update()
    {
        // Pursue();
        Hide();
    }
}
