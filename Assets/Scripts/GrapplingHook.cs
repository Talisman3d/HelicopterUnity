using System;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{

    // Receiver of Grappling hook
    Rigidbody receiver_rb;
    string receiverTag;

    // Parent helicopter
    GameObject helicopter;
    string parentTag;

    // Components
    private SpringJoint springJoint;
    private LineRenderer lineRenderer;
    private FixedJoint fixedJoint;

    // General
    [SerializeField] float maxRopeLength = 150f;
    [SerializeField] float grapplingTimer = 4f;
    [SerializeField] float timerIfGrappled = 5f;
    float ropeLength;
    
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
        // calc timer and rope length
        grapplingTimer-=Time.deltaTime;
        ropeLength = Vector3.Distance(helicopter.transform.position,gameObject.transform.position);

        // Kill object if harpoon lifetime runs out
        if (grapplingTimer<0){
            if (isGrappled){
                // Also kill joints just in case, if grappled
                Destroy(springJoint);
                Destroy(fixedJoint);
            }
            Destroy(gameObject);
        }

        // If the rope has exceeded max length, kill it
        if(ropeLength>maxRopeLength){
            Destroy(gameObject);
        }

        // If the parent has been set, draw line from parent to harpoon
        if (helicopter){
            lineRenderer.SetPosition(0, transform.position);  // Start point at object1
            lineRenderer.SetPosition(1, helicopter.transform.position);  // End point at object2
        }
       
    }

    public void initialization(GameObject heli, string shooterTag){
        // Set the helicopter game object who shot this harpoon
        helicopter = heli;
        // Set the controller of the helicopter type ("Player", "Friendly", "Enemy")
        parentTag=shooterTag;

    }

    private void OnCollisionEnter(Collision other) {
        receiverTag = other.gameObject.tag;

        // Allow some more time if they actually got grappled
        grapplingTimer = timerIfGrappled;

        // Latch on to only opposite team
        switch (parentTag){
            case "Player":
                switch (receiverTag){
                        case "Enemy":
                            latchOn(other.gameObject);
                            break;
                        case "Target":
                         
                            latchOn(other.gameObject);
                            break;
                        default:
                            break;
                }
                break;
            case "Friendly":
                switch (receiverTag){
                    case "Enemy":
                        latchOn(other.gameObject);
                        break;
                    case "Target":
                        latchOn(other.gameObject);
                    break;
                    default:
                        break;
                }
                break;
            case "Enemy":
                switch (receiverTag){
                    case "Enemy":
                        break;
                    default:
                        latchOn(other.gameObject);
                        break;
                }
                break;
            default:
                break;
        }
    }

    void latchOn(GameObject targetObject){

        // Fix harpoon to target
        fixedJoint=gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = targetObject.GetComponent<Rigidbody>();
        receiver_rb = targetObject.GetComponent<Rigidbody>();
        
        isGrappled = true;

        // and create a rope to original heli
        springJoint=gameObject.AddComponent<SpringJoint>();
        springJoint.connectedBody = helicopter.GetComponent<Rigidbody>();
        springJoint.spring = 50f;
        springJoint.damper = 5f;
        
        // Have it real in a little bit
        springJoint.maxDistance = ropeLength;
        springJoint.minDistance = ropeLength - 1f;
    }
}
