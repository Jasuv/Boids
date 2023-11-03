using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoidScriptExample : MonoBehaviour
{
    public Transform target;
    public float boidVision;
    public float CohesionWeight;
    public float boidAvoidDist;
    public float SeperationWeight;
    public float AlignmentWeight;
    public float obstAvoidDist;
    public float ObstAvoidWeight;
    public float AlgorithmPriority;
    public float velocLimit;
    public bool debugTime = false;
    public Vector3 initialVelocity;
    public Rigidbody rig;
    public Collider[] closeBoids;
    public LayerMask boidLayer;
    public int randNum;


    void Start()
    {
        boidLayer = LayerMask.GetMask("BoidLayer");
        rig = GetComponent<Rigidbody>();
        rig.velocity = initialVelocity;
        initialVelocity = new Vector3(Random.Range(-velocLimit, velocLimit),
                                      Random.Range(-velocLimit, velocLimit),
                                      Random.Range(-velocLimit, velocLimit));
        randNum = Random.Range(1, 4);
    }

    void FixedUpdate()
    {
        closeBoids = Physics.OverlapSphere(transform.position, boidVision, boidLayer);
        transform.forward = rig.velocity.normalized;
        Vector3 newVel = CapVelocity(CalculateVelocity(), velocLimit);
        rig.AddForce(newVel);
        rig.velocity = CapVelocity(rig.velocity, velocLimit);
    }

    private Vector3 CapVelocity(Vector3 velocity, float velocityLimit)
    {
        float velMagnitude = velocity.magnitude;

        if (velMagnitude > velocityLimit)
        {
            return (velocity / velMagnitude) * velocityLimit;
        }
        else
        {
            return velocity;
        }
    }

    private Vector3 BoidAlgorithm()
    {
        Vector3 seperationVector = Vector3.zero;
        Vector3 cohesionVector = Vector3.zero;
        Vector3 alignmentVector = Vector3.zero;

        if (closeBoids != null && closeBoids.Length > 0)
        {
            foreach (Collider boid in closeBoids)
            {
                if (Vector3.Distance(transform.position, boid.transform.position) < boidAvoidDist)
                {
                    seperationVector += transform.position - boid.transform.position;
                    if (debugTime == true)
                        Debug.DrawLine(transform.position, boid.transform.position, Color.yellow);
                }

                cohesionVector += boid.transform.position - transform.position;
                alignmentVector += boid.GetComponent<Rigidbody>().velocity;
            }
            cohesionVector /= closeBoids.Length;
            alignmentVector /= closeBoids.Length;
        }

        return (seperationVector * SeperationWeight) + (cohesionVector * CohesionWeight) + (alignmentVector * AlignmentWeight);
    }

    private Vector3 AvoidObstacles()
    {
        RaycastHit hit;
        Vector3 ObstAvoidVector = Vector3.zero;

        //forward
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstAvoidDist)) 
        {
            if (Physics.Raycast(transform.position, -transform.right, out hit, obstAvoidDist))
            {
                ObstAvoidVector += -transform.forward;
                ObstAvoidVector += transform.right;
            }
            else if (Physics.Raycast(transform.position, transform.right, out hit, obstAvoidDist))
            {
                ObstAvoidVector += -transform.forward;
                ObstAvoidVector += -transform.right;
            }
            else if (Physics.Raycast(transform.position, transform.up, out hit, obstAvoidDist))
            {
                ObstAvoidVector += -transform.forward;
                ObstAvoidVector += -transform.up;
            }
            else if (Physics.Raycast(transform.position, -transform.up, out hit, obstAvoidDist))
            {
                ObstAvoidVector += -transform.forward;
                ObstAvoidVector += transform.up;
            }
            else
            {
                if (randNum == 1)
                {
                    ObstAvoidVector += -transform.forward;
                    ObstAvoidVector += transform.right;
                }
                else if (randNum == 2)
                {
                    ObstAvoidVector += -transform.forward;
                    ObstAvoidVector += -transform.right;
                }
                else if (randNum == 3)
                {
                    ObstAvoidVector += -transform.forward;
                    ObstAvoidVector += -transform.up;
                }
                else if (randNum == 4)
                {
                    ObstAvoidVector += -transform.forward;
                    ObstAvoidVector += transform.up;
                }
            }
        }

        //back
        if (Physics.Raycast(transform.position, -transform.forward, out hit, obstAvoidDist))
            ObstAvoidVector += transform.forward;

        //left
        if (Physics.Raycast(transform.position, -transform.right, out hit, obstAvoidDist))
            ObstAvoidVector += transform.right;

        //right
        if (Physics.Raycast(transform.position, transform.right, out hit, obstAvoidDist))
            ObstAvoidVector += -transform.right;

        //up
        if (Physics.Raycast(transform.position, transform.up, out hit, obstAvoidDist))
            ObstAvoidVector += -transform.up;

        //down
        if (Physics.Raycast(transform.position, -transform.up, out hit, obstAvoidDist))
            ObstAvoidVector += transform.up;

        if (debugTime == true)
            Debug.DrawRay(transform.position, ObstAvoidVector, Color.cyan);

        return ObstAvoidVector * ObstAvoidWeight;
    }

    private Vector3 MoveToTarget()
    {
        if (target != null)
            return target.position - transform.position;
        return Vector3.zero;
    }

    private Vector3 CalculateVelocity()
    {
        Vector3 avoidVector = AvoidObstacles();

        if (!(avoidVector == Vector3.zero))
        {
            return avoidVector + (BoidAlgorithm() * AlgorithmPriority);
        }
        else if (closeBoids != null && closeBoids.Length == 0)
        {
            return rig.velocity;
        }
        else
        {
            return BoidAlgorithm() + MoveToTarget();
        }

    }
}
