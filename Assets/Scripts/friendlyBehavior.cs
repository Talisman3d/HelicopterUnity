using System;
using Unity.VisualScripting;
using UnityEngine;

public class friendlyBehavior : MonoBehaviour
{
    GameObject helicopter;

    FlightController flightController;

        // PID controllers for pitch, yaw, and thrust (lift)
    private PIDController pitchPID = new PIDController();
    private PIDController yawPID = new PIDController();
    private PIDController liftPID = new PIDController();
    private PIDController rollPID = new PIDController();

    Vector3 targetLoc;
    Vector3 targetRot;
    float pitchError;
    float yawError;
    float liftError;
    float rollError;

    Vector3 hoverLoc;
    Vector3 hoverRot;

    float maxLateralDistance = 50f;
    float maxFrontalDistance = 50f;
    float maxAngle = 45f;

    [SerializeField] GameObject target;
    GameObject weaponMaster;

                // Set some arbitrary PID gains for each axis
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

    float rollOutput = 0f;
    float pitchOutput = 0f;
    float yawOutput = 0f;
    float liftOutput = 0f;
    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        hoverLoc=helicopter.transform.position;
        hoverRot=helicopter.transform.forward; 

        targetLoc= target.transform.position;
        targetRot = target.transform.eulerAngles;

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

    weaponMaster = helicopter.transform.Find("Weapons").gameObject;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        trackTarget(targetLoc, targetRot);
        lockedOn(hoverRot);
    }

    private void trackTarget(Vector3 targetLocation, Vector3 targetRotation){

        Vector3 angleDelta = targetLocation - helicopter.transform.position;

        // Pitch
        if (enablePitch){
            float pitchFromZero = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.right);
            float normalizedPitchFromZero = Mathf.Clamp(pitchFromZero / maxAngle, -1f, 1f);

            float angle2Target = Vector3.Angle(angleDelta, helicopter.transform.forward);
            float distance2Target = angleDelta.magnitude;
            float perpendicularDistance = distance2Target*Mathf.Cos(angle2Target*Mathf.Deg2Rad);
            float normalizedPerpendicularDistance =  Mathf.Clamp(perpendicularDistance / maxFrontalDistance, -1f, 1f);
            float weightedPitchInput = .6f * normalizedPerpendicularDistance + 0.4f * normalizedPitchFromZero;
            pitchOutput = -pitchPID.GetControlOutput(weightedPitchInput);
            flightController.ApplyPitch(pitchOutput);
        }


        // Lift
        if (enableLift){
            float liftError = targetLoc.y - helicopter.transform.position.y; // Altitude error
            liftOutput = liftPID.GetControlOutput(liftError);
            flightController.ApplyLift(liftOutput);
        }

        

        // Yaw
        if (enableYaw){
            float yawError = Vector3.SignedAngle(helicopter.transform.forward, targetRotation, Vector3.up); // Yaw angle error
            yawOutput = yawPID.GetControlOutput(yawError);
            flightController.ApplyYaw(yawOutput);
        }

        

        // Roll
        if (enableRoll){
            float rollFromZero  = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.forward);
            float normalizedRollFromZero = Mathf.Clamp(rollFromZero / maxAngle, -1f, 1f);
            Vector3 leftDirection = Vector3.Cross(helicopter.transform.forward, Vector3.up).normalized;
            float lateralDistance = Vector3.Dot(angleDelta,leftDirection);
            float normalizedDistance = Mathf.Clamp(lateralDistance / maxLateralDistance, -1f, 1f);
            float weightedRollInput = 0.6f * normalizedDistance + 0.4f * normalizedRollFromZero;
            rollOutput = -rollPID.GetControlOutput(weightedRollInput);
            flightController.ApplyRoll(rollOutput);
        }

    }

    private void lockedOn(Vector3 targetLoc){
        Vector3 angleDelta = targetLoc - helicopter.transform.position;

        float pitch = Vector3.SignedAngle(helicopter.transform.forward, angleDelta, helicopter.transform.right);
        float yaw = Vector3.SignedAngle(helicopter.transform.forward, angleDelta, Vector3.up);

        Debug.Log("pitch: " + pitch +  "yaw: " + yaw);

        if (pitch<15 && pitch>-15 && yaw< 15 && yaw > -15){
            weaponMaster.GetComponent<Weapons>().startGunSound();
            weaponMaster.GetComponent<Weapons>().fireGun();
        }
        else{
            weaponMaster.GetComponent<Weapons>().stopGunSound();
        }
    }

}
