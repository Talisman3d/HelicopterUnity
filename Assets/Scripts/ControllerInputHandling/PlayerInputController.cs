using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static GlobalConstants;

public class PlayerInputController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputAction LiftInput;
    [SerializeField] InputAction PitchInput;
    [SerializeField] InputAction YawInput;
    [SerializeField] InputAction RollInput;
    [SerializeField] InputAction ThrustInput;

    // References to objects and classes
    GameObject helicopter;
    public FlightController FlightController { get; protected set; }
    private void OnEnable() {
        LiftInput.Enable();
        PitchInput.Enable();
        YawInput.Enable();
        RollInput.Enable();
        ThrustInput.Enable();
    }

    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        FlightController= returnFlightControllerType(helicopter);        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        ProcessLift();
        ProcessPitch();
        ProcessYaw();
        ProcessRoll();
        ProcessThrust();
    }

    private void ProcessLift()
    {
        float pow = LiftInput.ReadValue<float>();
        if (pow != 0)
        {
            //HoverControl.TargetAltitude = helicopter.transform.position.y + (pow * 3f);
        }
        //if (!HoverMode)
        //{
            FlightController.ApplyLift(pow);
       // }
    }

    private void ProcessPitch()
    {
        float pitch = PitchInput.ReadValue<float>();
        //HoverControl.TargetForwardVelocity = pitch * 10f;

       // if (!HoverMode)
       // {
            FlightController.ApplyPitch(pitch);
        //}
    }

    private void ProcessRoll()
    {
        float roll = RollInput.ReadValue<float>();
        //HoverControl.TargetLateralVelocity = roll * 10f;
        //if (!HoverMode)
        //{
            FlightController.ApplyRoll(roll);
        //}
    }

    private void ProcessYaw()
    {
        float yaw = YawInput.ReadValue<float>();
        if (yaw != 0)
        {
           // HoverControl.TargetHeading = helicopter.transform.forward;
        }

        FlightController.ApplyYaw(yaw);
    }

    private void ProcessThrust()
    {
        float thrust = ThrustInput.ReadValue<float>();

        FlightController.ApplyThrust(thrust);
    }
}
