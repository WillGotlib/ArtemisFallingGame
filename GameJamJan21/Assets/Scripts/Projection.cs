using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour
{
    LineRenderer lineRenderer;
    BulletFire bulletFire;
    public int numPoints = 50;
    public float distBetweenPoints = 0.1f;
    
    public  LayerMask CollidableLayers;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        bulletFire = GetComponent<BulletFire>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.positionCount = (int) numPoints;
        List<Vector3> points = new List<Vector3>();
        Vector3 startingPosition = bulletFire.rb.position;
        Vector3 startingVelocity = bulletFire.rb.velocity;
        for (float t = 0; t < numPoints; t += distBetweenPoints)
        {
            Vector3 newPoint = startingPosition + t * startingVelocity;
            points.Add(newPoint);
            if (Physics.OverlapSphere(newPoint, 2, CollidableLayers).Length > 0)
            {
                lineRenderer.positionCount = points.Count;
                break;
            }
        }
        lineRenderer.SetPositions(points.ToArray());
    }
}
