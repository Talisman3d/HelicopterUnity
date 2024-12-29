using System;
using Unity.VisualScripting;
using UnityEngine;

// Class defines behavior of friendly AI
// The goal of this class is just to switch between AI behaviors.
// Right now the AI moves to a target, offset by a distance
// Once it arrives near this target offset, it aims at target and fires.
public class friendlyBehavior : MonoBehaviour
{
    GameObject helicopter; // Helicopter object child
    Vector3 targetLoc; // Target Location
    [SerializeField] public GameObject target; // Target game object
    GameObject weaponMaster; // Weapons Controller, to be reworked
    [SerializeField] float targetLocked = 15f; // Target is in cross hairs when pitch and yaw are between -targetLocked and targetLocked degrees

    MoveToTarget moveToTargetScript; // MoveToTargetScript
    AimAtTarget AimAtTargetScript; // MoveToTargetScript

    [SerializeField] float targetOffsetDistance = 10f; // How far from target to move to 
    [SerializeField] float arrivedBuffer =10f; // Radius around target offset to consider 'arrived'
    Vector3 targetOffset; // Distance offset from target to do shootin

    Boolean arrived = false; // Boolean that states whether helicopter has arrived in target location buffer zone
    Boolean previousMode = false; // Switch used to go from moving to aiming


    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        // Set Weapons Controller
        weaponMaster = helicopter.transform.Find("Weapons").gameObject;


        moveToTargetScript = GetComponent<MoveToTarget>();
        AimAtTargetScript = GetComponent<AimAtTarget>();

        moveToTargetScript.enabled = true; 
        AimAtTargetScript.enabled = false; 

    }

    void FixedUpdate()
    {

        target = findClosestTarget();

        targetOffset= calcOffset(); // Calculates a location between the helicopter and target so that helicopter can fire at target from distance
        arrived = calcHasArrived(targetOffset); // Have we arrived at target offset        

        if (!arrived){
            // If we haven't arrived yet
            if (previousMode==arrived){
                // If we were in target offset zone previously, we're now switching back to moving mode
                AimAtTargetScript.enabled=false;
                moveToTargetScript.enabled=true;
            }
            moveToTargetScript.setTarget(targetOffset); // Move helicopter towards this location
            weaponMaster.GetComponent<WeaponController>().stopGunSound();
        }
        else{
            // If we are in target offset zone
            if (previousMode==!arrived){
                // If we were previously in moving mode and we just arrived
                AimAtTargetScript.enabled=true;
                moveToTargetScript.enabled=false;
            }

            AimAtTargetScript.setTarget(target.transform.position);
            lockedOn(targetLoc);
        }

        previousMode=arrived;
    }

    private GameObject findClosestTarget()
    {
        // Find all Target GameObjects in the scene (you can use a tag or find all objects)
        GameObject[] targets = GameObject.FindGameObjectsWithTag("target");

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject t in targets)
        {
            float targetDistance = Vector3.Distance(transform.position, t.transform.position);

            // Only consider targets within a certain range (optional)
            if (targetDistance < closestDistance)
            {
                closestDistance = targetDistance;
                closestTarget = t;
            }
        }

        Debug.Log(closestDistance);
        weaponMaster.GetComponent<WeaponController>().target=closestTarget;
        return closestTarget;
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
