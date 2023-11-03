using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public ComputeShader boidMath;
    public StructBoid[] structBoids;
    private BoidScript boidScript;

    //I dont get this...
    const int threadGroupSize = 1024;

    void Start()
    {
        boidScript = GetComponent<BoidScript>();
    }

    void Update()
    {
        if (boidScript.boids.Count != 0)
        {
            int boidCount = boidScript.boids.Count;
            structBoids = new StructBoid[boidCount];

            for (int i = 0; i < boidScript.boids.Count; i++)
            {
                structBoids[i].pos = boidScript.boids[i].pos;
                structBoids[i].veloc = boidScript.boids[i].veloc;
            }

            var boidBuffer = new ComputeBuffer(boidCount, StructBoid.Size);
            boidBuffer.SetData(structBoids);

            boidMath.SetBuffer(0, "boids", boidBuffer);
            boidMath.SetInt("size", boidCount);
            boidMath.SetFloat("viewRadius", boidScript.viewRadius);
            boidMath.SetFloat("avoidDist", boidScript.boidAvoidDist);
            boidMath.SetFloat("cohesionWeight", boidScript.CohesionWeight);
            boidMath.SetFloat("repulsionWeight", boidScript.RepulsionWeight);
            boidMath.SetFloat("alignmentWeight", boidScript.AlignmentWeight);

            int threadGroups = Mathf.CeilToInt(boidCount / (float)threadGroupSize);
            boidMath.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(structBoids);

            for (int i = 0; i < boidCount; i++)
            {
                boidScript.boids[i].cohesion = structBoids[i].cohesion;
                boidScript.boids[i].repulsion = structBoids[i].repulsion;
                boidScript.boids[i].alignment = structBoids[i].alignment;
            }

            boidBuffer.Dispose();
        }
    }

    public struct StructBoid
    {
        public Vector3 pos;
        public Vector3 veloc;

        public Vector3 cohesion;
        public Vector3 repulsion;
        public Vector3 alignment;

        //I have no fucking clue what the purpose of this is...
        public static int Size
        {
            get
            {
                return sizeof(float) * 14 + sizeof(int);
            }
        }
    }
}
