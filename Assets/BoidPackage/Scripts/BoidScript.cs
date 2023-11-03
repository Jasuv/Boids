using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid
{
    public GameObject body;
    public Vector3 pos;
    public Vector3 veloc;

    public Vector3 cohesion;
    public Vector3 repulsion;
    public Vector3 alignment;

    public Boid(Vector3 iPos, Vector3 iVeloc, Vector3 iCohesion, Vector3 iRepulsion, Vector3 iAlignment) 
    {
        pos = iPos;
        veloc = iVeloc;
    }
}

public class BoidScript : MonoBehaviour
{
    public Transform target;
    public float CohesionWeight;
    public float boidAvoidDist;
    public float RepulsionWeight;
    public float AlignmentWeight;
    public float obstAvoidDist;
    public float ObstAvoidWeight;
    public bool debugger = false;
    public LayerMask boidLayer;

    public List<Boid> boids = new List<Boid>();
    public int spawnCount;
    public int spawnRadius;
    public float viewRadius;
    public float velocityCap;
    public GameObject boidBody;

    private BoidManager boidManager;


    void Start()
    {
        boidManager = GetComponent<BoidManager>();
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newBoidBody = Instantiate(boidBody, Random.insideUnitSphere * spawnRadius, transform.rotation);
            Boid newBoid = new Boid(newBoidBody.transform.position, new Vector3(Random.value, Random.value, Random.value), Vector3.zero, Vector3.zero, Vector3.zero);
            newBoid.body = newBoidBody;
            boids.Add(newBoid);
        }
        print(boids.Count);
    }

    void Update()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            if ((boids[i].cohesion != Vector3.zero) && (boids[i].repulsion != Vector3.zero) && (boids[i].alignment != Vector3.zero))
            {
                if (ObstacleAvoidance(boids[i]) != Vector3.zero)
                {
                    boids[i].veloc += (ObstacleAvoidance(boids[i]) + MoveToTarget(boids[i]));
                }
                else
                {
                    boids[i].veloc += (boids[i].cohesion + boids[i].repulsion + boids[i].alignment + ObstacleAvoidance(boids[i]) + MoveToTarget(boids[i]));
                }
                boids[i].veloc = CapVelocity(boids[i].veloc, velocityCap);
                boids[i].pos += boids[i].veloc;
                boids[i].body.transform.position = boids[i].pos;
                boids[i].body.transform.forward = boids[i].veloc.normalized;
            }
            else
            {
                Debug.LogWarning("i fcked something up");
            }
        }
    }

    Vector3 ObstacleAvoidance(Boid iBoid)
    {
        RaycastHit hit;
        Vector3 ObstAvoidVector = Vector3.zero;

        //forward
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.forward, out hit, obstAvoidDist))
        {
            ObstAvoidVector += -iBoid.body.transform.forward;
            if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.right; }
            else if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.right; }
            else if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.up; }
            else if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.up; }
            else
            {
                int num = Random.Range(1, 4);
                if (num == 1) { ObstAvoidVector += iBoid.body.transform.right; }
                else if (num == 2) { ObstAvoidVector += -iBoid.body.transform.right; }
                else if (num == 3) { ObstAvoidVector += -iBoid.body.transform.up; }
                else if (num == 4) { ObstAvoidVector += iBoid.body.transform.up; }
            }
        }

        //back
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.forward, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.forward; }

        //left
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.right; }

        //right
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.right; }

        //up
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.up; }

        //down
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.up; }

        if (debugger == true) { Debug.DrawRay(iBoid.body.transform.position, ObstAvoidVector, Color.cyan); }

        return (ObstAvoidVector * ObstAvoidWeight);
    }

    private Vector3 MoveToTarget(Boid iboid)
    {
        if (target != null) { return target.position - iboid.pos; }
        return Vector3.zero;
    }

    Vector3 CapVelocity(Vector3 veloc, float cap)
    {
        if (veloc.magnitude > cap)
        {
            Vector3 newVeloc = veloc.normalized;
            newVeloc *= cap;
            return newVeloc;
        }
        else
        {
            return veloc;
        }
    }
}




























/*public class Boid
{
    public GameObject body;
    public Vector3 pos;
    public Vector3 veloc;
    public float viewRad;

    public Boid(Vector3 iPos, Vector3 iVeloc, float iViewRad) 
    {
        pos = iPos;
        veloc = iVeloc;
        viewRad = iViewRad;
    }
}

public class BoidScript: MonoBehaviour
{
    public Transform target;
    public float CohesionWeight;
    public float boidAvoidDist;
    public float RepulsionWeight;
    public float AlignmentWeight;
    public float obstAvoidDist;
    public float ObstAvoidWeight;
    public bool debugger = false;
    public LayerMask boidLayer;

    public List<Boid> boids = new List<Boid>();
    public int spawnCount;
    public int spawnRadius;
    public float viewRadius;
    public float velocityCap;
    public GameObject boidBody;

    private BoidManager boidManager;


    void Start()
    {
        boidManager = GetComponent<BoidManager>();
        for(int i = 0; i < spawnCount; i++)
        {
            GameObject newBoidBody = Instantiate(boidBody, Random.insideUnitSphere * spawnRadius, transform.rotation);
            Boid newBoid = new Boid(newBoidBody.transform.position, new Vector3(Random.value, Random.value, Random.value), viewRadius);
            newBoid.body = newBoidBody;
            boids.Add(newBoid);
        }
        boidManager.setupStructBoids();
    }

    void Update()
    {
        foreach (Boid boid in boids)
        {
            if (ObstacleAvoidance(boid) != Vector3.zero)
            {
                boid.veloc += (Cohesion(boid) / 100 + Repulsion(boid) / 100 + Alignment(boid) / 100 + ObstacleAvoidance(boid) + MoveToTarget(boid));
            }
            else 
            {
                boid.veloc += (Cohesion(boid) + Repulsion(boid) + Alignment(boid) + ObstacleAvoidance(boid) + MoveToTarget(boid));
            }
            boid.veloc = CapVelocity(boid.veloc, velocityCap);
            boid.pos += boid.veloc;
        }
        foreach (Boid boid in boids)
        {
            boid.body.transform.position = boid.pos;
            boid.body.transform.forward = boid.veloc.normalized;
        }
    }

    Vector3 Cohesion(Boid iBoid) 
    {
        float centroidX = 0;
        float centroidY = 0;
        float centroidZ = 0;
        foreach (Boid boid in boids)
        {
            centroidX += boid.pos.x;
            centroidY += boid.pos.y;
            centroidZ += boid.pos.z;
        }
        centroidX /= boids.Count;
        centroidY /= boids.Count;
        centroidZ /= boids.Count;
        Vector3 centroid = new Vector3(centroidX, centroidY, centroidZ);
        return (centroid - iBoid.pos) * CohesionWeight;
    }

    Vector3 Repulsion(Boid iBoid) 
    {
        int avoidDist = 2;
        Vector3 avoidVect = new Vector3();
        foreach (Boid otherBoid in boids)
        {
            if (iBoid != otherBoid)
            {
                if (Distance(iBoid.pos, otherBoid.pos) < avoidDist)
                {
                    avoidVect += iBoid.pos - otherBoid.pos;
                }
            }
        }
        return avoidVect * RepulsionWeight;
    }

    Vector3 Alignment(Boid iBoid) 
    {
        Vector3 avgVeloc = new Vector3();
        foreach (Boid boid in boids)
        {
            avgVeloc += boid.veloc;
        }
        avgVeloc /= spawnCount;
        return (avgVeloc + iBoid.veloc) * AlignmentWeight;
    }

    Vector3 ObstacleAvoidance(Boid iBoid) 
    {
        RaycastHit hit;
        Vector3 ObstAvoidVector = Vector3.zero;

        //forward
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.forward, out hit, obstAvoidDist))
        {
            ObstAvoidVector += -iBoid.body.transform.forward;
            if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.right; }
            else if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.right; }
            else if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.up; }
            else if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.up; }
            else
            {
                int num = Random.Range(1, 4);
                if (num == 1) { ObstAvoidVector += iBoid.body.transform.right; }
                else if (num == 2) { ObstAvoidVector += -iBoid.body.transform.right; }
                else if (num == 3) { ObstAvoidVector += -iBoid.body.transform.up; }
                else if (num == 4) { ObstAvoidVector += iBoid.body.transform.up; }
            }
        }

        //back
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.forward, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.forward; }

        //left
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.right; }

        //right
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.right, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.right; }

        //up
        if (Physics.Raycast(iBoid.body.transform.position, iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += -iBoid.body.transform.up; }

        //down
        if (Physics.Raycast(iBoid.body.transform.position, -iBoid.body.transform.up, out hit, obstAvoidDist)) { ObstAvoidVector += iBoid.body.transform.up; }

        if (debugger == true) { Debug.DrawRay(iBoid.body.transform.position, ObstAvoidVector, Color.cyan); }

        return (ObstAvoidVector * ObstAvoidWeight);
    }

    private Vector3 MoveToTarget(Boid iboid)
    {
        if (target != null) { return target.position - iboid.pos; }
        return Vector3.zero;
    }

    float Distance(Vector3 pos1, Vector3 pos2)
    {
        float dist = 0;
        float distX = Mathf.Pow((pos2.x - pos1.x), 2);
        float distY = Mathf.Pow((pos2.y - pos1.y), 2);
        float distZ = Mathf.Pow((pos2.z - pos1.z), 2);
        dist = Mathf.Sqrt(distX + distY + distZ);
        return dist;
    }

    Vector3 CapVelocity(Vector3 veloc, float cap)
    {
        if (veloc.magnitude > cap)
        {
            Vector3 newVeloc = veloc.normalized;
            newVeloc *= cap;
            return newVeloc;
        }
        else
        {
            return veloc;
        }
    }
}*/

