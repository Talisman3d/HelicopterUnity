using System;
using Unity.VisualScripting;
using UnityEngine;


// Class defines behavior of friendly AI
// The goal of this class is just to switch between AI behaviors.
// List of potential behaviors:
// # Flight
// - Hover
// - Move To position
// - Follow
// - Patrol
// # Combat
// - Standoff Attack (missiles)
// - Aggressive Attack (guns/rockets)
// - Strafing Run (guns/rockets)
// - Retreat
public class friendlyBehavior : MonoBehaviour
{
    [Header("Target Setup")]
    Vector3 targetLoc; // Target Location
    [SerializeField] public GameObject target; // Target game object
    [SerializeField] float targetLocked = 15f; // Target is in cross hairs when pitch and yaw are between -targetLocked and targetLocked degrees
    [SerializeField] float targetOffsetDistance = 10f; // How far from target to move to 
    Vector3 targetOffset; // Distance offset from target to do shootin


    // References to other objects
    GameObject helicopter; // Helicopter object child
    GameObject weaponMaster; // Weapons Controller, to be reworked
    
    // References to other scripts
    AutoPilot autoPilotScript; // MoveToTargetScript
    AimAtTarget AimAtTargetScript; // MoveToTargetScript
    FindClosestTarget findClosestTarget; // script to scan for closest target

    // Arrival settings
    [SerializeField] float arrivedBuffer =10f; // Radius around target offset to consider 'arrived'
    Boolean arrived = false; // Boolean that states whether helicopter has arrived in target location buffer zone

    // Mode switching boolean
    Boolean previousMode = false; // Switch used to go from moving to aiming


    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        // Set Weapons Controller
        weaponMaster = helicopter.transform.Find("Weapons").gameObject;

        // Reference and initialize other behavior scripts
        autoPilotScript = GetComponent<AutoPilot>();
        autoPilotScript.enabled = true; 

        AimAtTargetScript = GetComponent<AimAtTarget>();        
        AimAtTargetScript.enabled = false; 

        findClosestTarget = helicopter.GetComponent<FindClosestTarget>();
        findClosestTarget.enabled = true;
    }

    void FixedUpdate()
    {

        target = findClosestTarget.closestTarget;

        if (target){
            targetOffset= calcOffset(); // Calculates a location between the helicopter and target so that helicopter can fire at target from distance
            arrived = calcHasArrived(targetOffset); // Have we arrived at target offset        

            if (!arrived){
                // If we haven't arrived yet
                if (previousMode==arrived){
                    // If we were in target offset zone previously, we're now switching back to moving mode
                    AimAtTargetScript.enabled=false;
                    autoPilotScript.MoveToTarget(target);
                }
                //flyToTargetScript.setTarget(targetOffset); // Move helicopter towards this location
                weaponMaster.GetComponent<WeaponController>().stopGunSound();
            }
            else{
                // If we are in target offset zone
                if (previousMode==!arrived){
                    // If we were previously in moving mode and we just arrived
                    AimAtTargetScript.enabled=true;
                    autoPilotScript.HoverInPlace();
                }

                AimAtTargetScript.setTarget(target.transform.position);
                lockedOn(targetLoc);
            }

            previousMode=arrived;
        }
    }

    private void lockedOn(Vector3 targetLoc){
        // Fire on target when helicopter is facing target
        
        Vector3 VectorDelta = targetLoc - helicopter.transform.position; // Vector difference from helicopter to target

        float pitch = Vector3.SignedAngle(helicopter.transform.forward, VectorDelta, helicopter.transform.right); // Pitch angle diff of helicopter to target
        float yaw = Vector3.SignedAngle(helicopter.transform.forward, VectorDelta, Vector3.up); // Yaw angle diff of helicopter to target

        if (pitch<targetLocked && pitch>-targetLocked && yaw< targetLocked && yaw > -targetLocked){
            // If within angle cone. Might add distance eventually
            weaponMaster.GetComponent<WeaponController>().startGunSound();
            weaponMaster.GetComponent<WeaponController>().fireGun();
        }
        else{
            weaponMaster.GetComponent<WeaponController>().stopGunSound();
        }
    }

    private Vector3 calcOffset(){
        // Calculates a location between the helicopter and target so that helicopter can fire at target from distance
        Vector3 VectorDelta = target.transform.position - new Vector3(helicopter.transform.position.x,target.transform.position.y,helicopter.transform.position.z); // Vector difference from helicopter to target
        Vector3 deltaDirection = VectorDelta.normalized;
        Vector3 tOffset = target.transform.position - deltaDirection * targetOffsetDistance;      
        return tOffset;
    }

    private Boolean calcHasArrived(Vector3 targetVectorLocation){
        // Are we within target area zone
        Vector3 VectorDelta = targetVectorLocation - helicopter.transform.position; // Vector difference from helicopter to target
        
        if (VectorDelta.magnitude<arrivedBuffer){
            return true;
        }
        else{
            return false;
        }
    }

}
