using UnityEngine;

public class AimAtTarget : MonoBehaviour
{
    Vector3 targetLoc;


    private PIDController pitchPID = new PIDController();
    private PIDController yawPID = new PIDController();
    private PIDController liftPID = new PIDController();
    private PIDController rollPID = new PIDController();

    float pitchError;
    float yawError;
    float liftError;
    float rollError;

                    // Set some arbitrary PID gains for each axis
    [SerializeField] float pitchPIDKp = .01f;
    [SerializeField] float pitchPIDKi = .01f;
    [SerializeField] float pitchPIDKd = 0.1f;

    [SerializeField] float yawPIDKp = 2f;
    [SerializeField] float yawPIDKi = 0.1f;
    [SerializeField] float yawPIDKd = 0.1f;

    [SerializeField] float   liftPIDKp = 5f;
    [SerializeField] float liftPIDKi = 0.3f;
    [SerializeField] float liftPIDKd = .1f;

    [SerializeField] float rollPIDKp = .04f;
    [SerializeField] float rollPIDKi = 0.1f;
    [SerializeField] float rollPIDKd = 0.1f;

    float rollOutput = 0f;
    float pitchOutput = 0f;
    float yawOutput = 0f;
    float liftOutput = 0f;

    GameObject helicopter;

    FlightController flightController;

    void Start()
    {
         // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        targetLoc= helicopter.transform.position;

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
        }

   void FixedUpdate()
    {
        
        aimTowardsTarget(targetLoc);
        Debug.DrawLine(helicopter.transform.position, targetLoc, Color.red, 0.1f);
    }

    public void setTarget(Vector3 targetLocation){
        targetLoc=targetLocation;
    }

    private void aimTowardsTarget(Vector3 targetLocation){
        // WIP, movement is horrible right now
        // Restricts behavior to rotor physics limitations and tries to move helicopter to target
        // Aproach now is to wait for AI to yaw relatively at target, then can roll to adjust lateral distance, and pitch to move towards target

        Vector3 vectorDelta = targetLocation - helicopter.transform.position; // Vector from helicopter to target

        // Pitch
        // Aim up or down at target
        pitchError = Vector3.SignedAngle(helicopter.transform.forward, targetLocation, helicopter.transform.right); // pitch angle to target
        pitchOutput = -pitchPID.GetControlOutput(pitchError);
        
        // Lift
        // Move to altitude of target
        liftError = targetLoc.y - helicopter.transform.position.y; // Altitude error
        liftOutput = liftPID.GetControlOutput(liftError);
        flightController.ApplyLift(liftOutput);
    

        // Yaw
        // Aim left or right at target
        yawError = Vector3.SignedAngle(helicopter.transform.forward, vectorDelta, Vector3.up); // Yaw angle error
        //yawOutput = yawPID.GetControlOutput(yawError);
        yawOutput= Mathf.Clamp(yawError / 30, -1f, 1f); //No PID, just linear correction
        flightController.ApplyYaw(yawOutput);

        // Roll
        // Try not to roll over
        float rollFromZero  = Vector3.SignedAngle(helicopter.transform.up, Vector3.up, helicopter.transform.forward);
        rollOutput = -rollPID.GetControlOutput(rollFromZero);
        flightController.ApplyRoll(rollOutput);

    }
}
