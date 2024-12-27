using System;
using Unity.VisualScripting;
using UnityEngine;

// Class defines behavior of friendly AI
// WIP, to be refactored
// The goal of this class is just to switch between behaviors, currently it holds MoveToTarget
public class friendlyBehavior : MonoBehaviour
{
    GameObject helicopter; // Helicopter object child

    FlightController flightController; // Flight Controller, interprets inputs to rotor thrust

    // PID controllers for pitch, yaw, roll, and thrust (lift)
    private PIDController pitchPID = new PIDController();
    private PIDController yawPID = new PIDController();
    private PIDController liftPID = new PIDController();
    private PIDController rollPID = new PIDController();

    Vector3 targetLoc; // Target Location
    Vector3 targetRot; // Target Rotation

    // Errors for each PID
    float pitchError;
    float yawError;
    float liftError;
    float rollError;

    float maxLateralDistance = 50f; // Needed to clamp roll error
    float maxFrontalDistance = 50f; // Needed to clamp pitch error
    float maxAngle = 30f; // Safety net roll/pitch angle

    [SerializeField] GameObject target; // Target game object
    GameObject weaponMaster; // Weapons Controller, to be reworked

    // PID gains
    [SerializeField] Boolean enablePitch = true;
    [SerializeField] float pitchPIDKp = .01f;
    [SerializeField] float pitchPIDKi = .01f;
    [SerializeField] float pitchPIDKd = 0.1f;

    [SerializeField] Boolean enableYaw = true;
    [SerializeField] float yawPIDKp = 2f;
    [SerializeField] float yawPIDKi = 0.1f;
    [SerializeField] float yawPIDKd = 0.1f;

    [SerializeField] Boolean enableLift = true;
    [SerializeField] float   liftPIDKp = 5f;
    [SerializeField] float liftPIDKi = 0.3f;
    [SerializeField] float liftPIDKd = .1f;

    [SerializeField] Boolean enableRoll = true;
    [SerializeField] float rollPIDKp = .04f;
    [SerializeField] float rollPIDKi = 0.1f;
    [SerializeField] float rollPIDKd = 0.1f;

    // PID outputs
    float rollOutput = 0f;
    float pitchOutput = 0f;
    float yawOutput = 0f;
    float liftOutput = 0f;

    float targetLocked = 5f; // Target is in cross hairs when pitch and yaw are between -targetLocked and targetLocked degrees
    float yawBounds = 15f; // Angle boundary in which roll and pitch can begin

    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        // For now, setting target's initial location and rotation as our target
        targetLoc= target.transform.position;
        targetRot = target.transform.eulerAngles;

        // Surely there's a better way....
        if (helicopter.CompareTag("tandem"))
        {
            flightController = helicopter.GetComponent<TandemFlightController>();
        }
        else if (helicopter.CompareTag("conventional"))
        {
            flightController = helicopter.GetComponent<ConventionalFlightController>();
        }
        else if (helicopter.CompareTag("coax"))
        {
            flightController = helicopter.GetComponent<CoaxFlightController>();
        }
        else if (helicopter.CompareTag("quad"))
        {
            flightController = helicopter.GetComponent<QuadFlightController>();
        }
        else if (helicopter.CompareTag("sidebyside"))
        {
            flightController = helicopter.GetComponent<SideBySideFlightController>();
        }

    // Set Inspector gains as actual PID gains
    pitchPID.Kp = pitchPIDKp;
    pitchPID.Ki = pitchPIDKi;
    pitchPID.Kd = pitchPIDKd;

    yawPID.Kp = yawPIDKp;
    yawPID.Ki = yawPIDKi;
    yawPID.Kd = yawPIDKd;

    liftPID.Kp = liftPIDKp;
    liftPID.Ki = liftPIDKi;
    liftPID.Kd = liftPIDKd;

    rollPID.Kp = rollPIDKp;
    rollPID.Ki = rollPIDKi;
    rollPID.Kd = rollPIDKd;

    // Set Weapons Controller
    weaponMaster = helicopter.transform.Find("Weapons").gameObject;

    }

    void FixedUpdate()
    {
        targetLoc = target.transform.position;
        trackTarget(targetLoc); // Move to target
        //lockedOn(targetLoc); // Fire at target when yaw, pitch angles are lined up
    }

    private void trackTarget(Vector3 targetLocation){
        // WIP, movement is horrible right now
        // Restricts behavior to rotor physics limitations and tries to move helicopter to target
        // Aproach now is to wait for AI to yaw relatively at target, then can roll to adjust lateral distance, and pitch to move towards target

        Vector3 vectorDelta = targetLocation - helicopter.transform.position; // Vector from helicopter to target

        // Pitch
        // WIP
        if (enablePitch){
            if (Mathf.Abs(yawError)<yawBounds){
                // If Helicopter is facing relatively towards target
                // Use a combination of distance towards target and pitch from global up to ensure no tipping
                float pitchFromZero = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.right); // Helicopter pitch angle from global up WRT helicopter right
                float normalizedPitchFromZero = Mathf.Clamp(pitchFromZero / maxAngle, -1f, 1f); // Clamp helicopter pitch angle at 45 to normalize
                float angle2Target = Vector3.Angle(vectorDelta, helicopter.transform.forward); // Euler angles of VectorDelta
                float distance2Target = vectorDelta.magnitude; // Distance of vectorDelta
                float perpendicularDistance = distance2Target*Mathf.Cos(angle2Target*Mathf.Deg2Rad); // Helicopter forward to perpendicular distance of target
                float normalizedPerpendicularDistance =  Mathf.Clamp(perpendicularDistance / maxFrontalDistance, -1f, 1f); // Normalize perpendicular distance
                float weightedPitchInput = .3f * normalizedPerpendicularDistance + 0.7f * normalizedPitchFromZero; // Weighted combination of distance to perpendicular vector, and pitch from zero
                pitchOutput = -pitchPID.GetControlOutput(weightedPitchInput);
            }
            else{
                // If Helicopter is not facing towards target, just try not to tip over lol
                float pitchFromZero = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.right); // Helicopter pitch angle from global up WRT helicopter right
                pitchOutput = -pitchPID.GetControlOutput(pitchFromZero);
            }

            flightController.ApplyPitch(pitchOutput);
        }



        // Lift
        // Error is simple altitude difference of helicopter and target
        if (enableLift){
            liftError = targetLoc.y - helicopter.transform.position.y; // Altitude error
            liftOutput = liftPID.GetControlOutput(liftError);
            flightController.ApplyLift(liftOutput);
        }

        // Yaw
        // Error is simple angle difference of helicopter forward to target rotation WRT global up
        if (enableYaw){
            yawError = Vector3.SignedAngle(helicopter.transform.forward, vectorDelta, Vector3.up); // Yaw angle error
            //yawOutput = yawPID.GetControlOutput(yawError);
            yawOutput= Mathf.Clamp(yawError / 30, -1f, 1f); //No PID, just linear correction
            flightController.ApplyYaw(yawOutput);
        }

         Debug.DrawLine(helicopter.transform.position, targetLocation, Color.red, 0.1f);

        // Roll
        // Error is a combination of perpendicular distance to target and roll from zero to ensure no flipping
        if (enableRoll){
            if (Mathf.Abs(yawError)<yawBounds){
                // If helicopter is facing near target
                // Use a combination of lateral distance to target and roll angle to ensure no tipping
                float rollFromZero  = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.forward); // Roll angle from global up WRT Helicopter forward axis
                float normalizedRollFromZero = Mathf.Clamp(rollFromZero / maxAngle, -1f, 1f); // Normalize this by clamping at 45
                Vector3 leftDirection = Vector3.Cross(helicopter.transform.forward, Vector3.up).normalized; // Perpendicular vector of helicopter left
                float lateralDistance = Vector3.Dot(vectorDelta,leftDirection); // Distance of perpendicular left vector to target
                float normalizedDistance = Mathf.Clamp(lateralDistance / maxLateralDistance, -1f, 1f); // Normalize this distance by clamping at maxLateralDistance
                float weightedRollInput = 0.3f * normalizedDistance + 0.7f * normalizedRollFromZero; // Weighted combination of lateral distance to target and roll from zero angle
                rollOutput = -rollPID.GetControlOutput(weightedRollInput);
            }
            else{
                // If not facing towards target, just wait til helicopter yaws in general direction
                float rollFromZero  = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.forward);
                rollOutput = -rollPID.GetControlOutput(rollFromZero);
            }

            flightController.ApplyRoll(rollOutput);
        }

    }

    private void lockedOn(Vector3 targetLoc){
        // Fire on target when helicopter is facing target
        
        Vector3 angleDelta = targetLoc - helicopter.transform.position; // Vector difference from helicopter to target

        float pitch = Vector3.SignedAngle(helicopter.transform.forward, angleDelta, helicopter.transform.right); // Pitch angle diff of helicopter to target
        float yaw = Vector3.SignedAngle(helicopter.transform.forward, angleDelta, Vector3.up); // Yaw angle diff of helicopter to target

        if (pitch<targetLocked && pitch>-targetLocked && yaw< targetLocked && yaw > -targetLocked){
            // If within angle cone. Might add distance eventually
            weaponMaster.GetComponent<WeaponController>().startGunSound();
            weaponMaster.GetComponent<WeaponController>().fireGun();
        }
        else{
            weaponMaster.GetComponent<WeaponController>().stopGunSound();
        }
    }

}
