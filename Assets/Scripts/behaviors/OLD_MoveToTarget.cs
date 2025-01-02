using System;
using Unity.VisualScripting;
using UnityEngine;

// WIP, currently abstracting so that AI and player can use it
public class OLD_MoveToTarget : MonoBehaviour
{
    // References to other objects, scripts, positions, rotations
    GameObject helicopter;
    FlightController flightController;    
    Vector3 targetLoc;
    Vector3 targetRot;
    Vector3 hoverLoc;
    Vector3 hoverRot;

    // Setting max values for normalizations purposes
    float maxLateralDistance = 50f;
    float maxFrontalDistance = 50f;
    float maxAngle = 45f;

    // PID

    [Header("Pitch PID")]
    private PIDController pitchPID = new PIDController();
    [SerializeField] Boolean enablePitch = true;
    [SerializeField] float pitchPIDKp = .01f;
    [SerializeField] float pitchPIDKi = .01f;
    [SerializeField] float pitchPIDKd = 0.1f;
    float pitchOutput = 0f;
    float pitchError;

    [Header("Yaw PID")]
    private PIDController yawPID = new PIDController();
    [SerializeField] Boolean enableYaw = true;
    [SerializeField] float yawPIDKp = 2f;
    [SerializeField] float yawPIDKi = 0.1f;
    [SerializeField] float yawPIDKd = 0.1f;
    [SerializeField] float yawBounds = 15f; // Angle boundary in which roll and pitch can begin
    float yawOutput = 0f;
    float yawError;

    [Header("Lift PID")]
    private PIDController liftPID = new PIDController();
    [SerializeField] Boolean enableLift = true;
    [SerializeField] float   liftPIDKp = 5f;
    [SerializeField] float liftPIDKi = 0.3f;
    [SerializeField] float liftPIDKd = .1f;
    float liftOutput = 0f;
    float liftError;

    [Header("Roll PID")]
    private PIDController rollPID = new PIDController();
    [SerializeField] Boolean enableRoll = true;
    [SerializeField] float rollPIDKp = .04f;
    [SerializeField] float rollPIDKi = 0.1f;
    [SerializeField] float rollPIDKd = 0.1f;
    float rollOutput = 0f;
    float rollError;

    [Header("Thrust PID")]
    private PIDController thrustPID = new PIDController();
    [SerializeField] Boolean enableThrust = true;
    [SerializeField] float thrustPIDKp = .04f;
    [SerializeField] float thrustPIDKi = 0.1f;
    [SerializeField] float thrustPIDKd = 0.1f;
    float thrustOutput = 0f; 
    float thrustError;   

    
    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        hoverLoc=helicopter.transform.position;
        hoverRot=helicopter.transform.forward; 

        targetLoc= helicopter.transform.position;
        targetRot = helicopter.transform.eulerAngles;

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

    thrustPID.Kp = thrustPIDKp;
    thrustPID.Ki = thrustPIDKi;
    thrustPID.Kd = thrustPIDKd;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        trackTarget(targetLoc);
        Debug.DrawLine(helicopter.transform.position, targetLoc, Color.red, 0.1f);
    }

    public void setTarget(Vector3 targetLocation){
        targetLoc=targetLocation;
    }

    private void trackTarget(Vector3 targetLocation){
        // WIP, movement is horrible right now
        // Restricts behavior to rotor physics limitations and tries to move helicopter to target
        // Aproach now is to wait for AI to yaw relatively at target, then can roll to adjust lateral distance, and pitch to move towards target

        Vector3 vectorDelta = targetLocation - helicopter.transform.position; // Vector from helicopter to target

        // Pitch
        // WIP
        // For now I boosted weighted factor by 100
        if (enablePitch){
            if (Mathf.Abs(yawError)<yawBounds){
                // If Helicopter is facing relatively towards target
                // Use a combination of distance towards target and pitch from global up to ensure no tipping
                float pitchFromZero = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.right); // Helicopter pitch angle from global up WRT helicopter right
                float normalizedPitchFromZero = Mathf.Clamp(pitchFromZero / maxAngle, -1f, 1f); // Clamp helicopter pitch angle at 45 to normalize
                float distance2Target = vectorDelta.magnitude; // Distance of vectorDelta
                float normalizedDistance =  Mathf.Clamp(distance2Target / maxFrontalDistance, 0, 1f); // Normalize perpendicular distance to angle max
                float weightedPitchInput = (.5f * normalizedDistance + 0.5f * normalizedPitchFromZero)*100 ; // Weighted combination of distance to perpendicular vector, and pitch from zero
                pitchOutput = -pitchPID.GetControlOutput(weightedPitchInput);
                //Debug.Log("normalizedPitchFromZero: " + normalizedPitchFromZero+ " normalizedDistance: " + normalizedDistance);
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
            yawOutput = yawPID.GetControlOutput(yawError);
            //yawOutput= Mathf.Clamp(yawError / 30, -1f, 1f); //No PID, just linear correction
            flightController.ApplyYaw(yawOutput);
        }

         

        // Roll
        // Error is a combination of perpendicular distance to target and roll from zero to ensure no flipping
        if (enableRoll){
            if (Mathf.Abs(yawError)<yawBounds){
                // If helicopter is facing near target
                // Use a combination of lateral distance to target and roll angle to ensure no tipping
                float rollFromZero  = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.forward); // Roll angle from global up WRT Helicopter forward axis
                float normalizedRollFromZero = Mathf.Clamp(rollFromZero / maxAngle, -1f, 1f); // Normalize this by clamping at max angle
                Vector3 leftDirection = Vector3.Cross(helicopter.transform.forward, Vector3.up).normalized; // Perpendicular vector of helicopter left
                float lateralDistance = Vector3.Dot(vectorDelta,leftDirection); // Distance of perpendicular left vector to target
                float normalizedDistance = Mathf.Clamp(lateralDistance / maxLateralDistance, 0f, 1f); // Normalize this distance by clamping at maxLateralDistance
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

        // thrust
        // Error is simple distance difference from helicopter to target (currently only applies to coax)
        if (enableThrust){
            if (Mathf.Abs(yawError)<yawBounds){
                thrustError = (new Vector3(targetLocation.x,0,targetLocation.y) - new Vector3(helicopter.transform.position.x,0,helicopter.transform.position.y)).magnitude; // Vector from helicopter to target
                thrustOutput = -thrustPID.GetControlOutput(thrustError);
               
                flightController.ApplyThrust(thrustOutput);
            }
        }

    }

}
