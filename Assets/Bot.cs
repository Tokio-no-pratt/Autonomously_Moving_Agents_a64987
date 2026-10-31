
using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;
    Drive ds;

    Vector3 wanderTarget = Vector3.zero;

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

        float toTarget = Vector3.Angle(transform.forward, targetDir);

        if ((toTarget > 90 && relativeHeading < 20) || ds.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }

        float speed = agent.speed + ds.currentSpeed;
        float lookAhead = speed > 0.01f ? targetDir.magnitude / speed : 0f;

        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    void Evade()
    {
        Vector3 targetDir = target.transform.position - transform.position;

        float speed = agent.speed + ds.currentSpeed;
        float lookAhead = speed > 0.01f ? targetDir.magnitude / speed : 0f;

        Flee(target.transform.position + target.transform.forward * lookAhead);
    }

    void Wander()
    {
        float wanderRadius = 10;
        float wanderDistance = 20;
        float wanderJitter = 1;

        wanderTarget += new Vector3(
            Random.Range(-1.0f, 1.0f) * wanderJitter,
            0,
            Random.Range(-1.0f, 1.0f) * wanderJitter
        );

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);

        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        Seek(targetWorld);
    }

    void Hide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;

        GameObject chosenGo = null;

        GameObject[] hidingSpots = world.Instance.GetHidingSpots();

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hiderDir = hidingSpots[i].transform.position - target.transform.position;

            Vector3 hidePos = hidingSpots[i].transform.position + hiderDir.normalized * 10;

            if (Vector3.Distance(transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                chosenDir = hiderDir;
                chosenGo = hidingSpots[i];

                dist = Vector3.Distance(transform.position, hidePos);
            }
        }

        if (chosenGo != null)
        {
            Collider hideCol = chosenGo.GetComponent<Collider>();

            if (hideCol != null)
            {
                Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
                RaycastHit info;

                float distance = 100.0f;

                if (hideCol.Raycast(backRay, out info, distance))
                {
                    Seek(info.point + chosenDir.normalized * 5);
                }
                else
                {
                    Seek(chosenSpot);
                }
            }
        }
    }

    void CleverHide()
    {
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        GameObject[] hidingSpots = world.Instance.GetHidingSpots();

        for (int i = 0; i < hidingSpots.Length; i++)
        {
            Vector3 hiderDir = hidingSpots[i].transform.position - target.transform.position;

            Vector3 hidePos = hidingSpots[i].transform.position + hiderDir.normalized * 10;

            if (Vector3.Distance(transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                dist = Vector3.Distance(transform.position, hidePos);
            }
        }

        Seek(chosenSpot);
    }
    bool CanSeeTarget()
    {
        RaycastHit raycastInfo;
        Vector3 rayToTarget = target.transform.position - this.transform.position;
        float lookAngle = Vector3.Angle(this.transform.forward, rayToTarget);
        if(lookAngle < 60 && Physics.Raycast(this.transform.position,rayToTarget,out raycastInfo))
        {
            if (raycastInfo.transform.gameObject.tag == "cop")
                return true; 
        }
        return false;
    }

    bool CanSeeMe()
    {
        Vector3 rayToTarget = target.transform.position - this.transform.position;
        float lookAngle = Vector3.Angle(this.transform.forward, rayToTarget);
        if (lookAngle < 60)
            return true;
        return false;
    }
    bool coolDown = false;
    void BehaviourCooldown()
    {
        coolDown = false;
    }
    void Update()
    {
        if (CanSeeTarget() && CanSeeMe())
        {
            CleverHide();
        }
        else
            Pursue();
    }
}
