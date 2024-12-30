using System;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    // Receiver of Grappling hook
    Rigidbody receiver_rb;

    private SpringJoint springJoint;
    private LineRenderer lineRenderer;
    private FixedJoint fixedJoint;

    // Parent helicopter
    public GameObject helicopter;
    Rigidbody rb;


    // General
    float ropeLength;
    float grapplingTimer = 1000f;
    Boolean isGrappled = false;

    float lineWidth = 0.2f;

    void Start()
    {   
        lineRenderer=GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled=true;        
    }

    private void Update() {
        grapplingTimer--;
        if (grapplingTimer<0){
            if (isGrappled){
                Destroy(springJoint);
                Destroy(fixedJoint);
            }
            Destroy(gameObject);
        }

        if (helicopter){
            lineRenderer.SetPosition(0, transform.position);  // Start point at object1
            lineRenderer.SetPosition(1, helicopter.transform.position);  // End point at object2
        }
    }

    private void OnCollisionEnter(Collision other) {
        // If attached to a body, stick to that body
        
        fixedJoint=gameObject.AddComponent<FixedJoint>();
       
        fixedJoint.connectedBody = other.gameObject.GetComponent<Rigidbody>();
        receiver_rb = other.gameObject.GetComponent<Rigidbody>();
        
        
        isGrappled = true;

        // and create a rope to original heli
        springJoint=gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = helicopter.GetComponent<Rigidbody>();
        springJoint.spring = 50f;
        springJoint.damper = 5f;
        ropeLength = Vector3.Distance(helicopter.transform.position,gameObject.transform.position);
        springJoint.maxDistance = ropeLength;
        springJoint.minDistance = ropeLength - 0.1f;
    }
}
