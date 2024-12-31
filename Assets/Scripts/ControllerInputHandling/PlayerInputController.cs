using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputAction LiftInput;
    [SerializeField] InputAction PitchInput;
    [SerializeField] InputAction YawInput;
    [SerializeField] InputAction RollInput;
    [SerializeField] InputAction ThrustInput;
    [SerializeField] InputAction HoverModeInput;

    // References to objects and classes
    GameObject helicopter;
    public FlightController FlightController { get; protected set; }
    public bool HoverMode { get; protected set; }
    public HoverControlComponent HoverControl { get; protected set; }

    private void OnEnable() {
        LiftInput.Enable();
        PitchInput.Enable();
        YawInput.Enable();
        RollInput.Enable();
        ThrustInput.Enable();

        HoverModeInput.Enable();
        HoverModeInput.performed += ToggleHoverMode;
    }

    void Start()
    {
        // Assuming helicopter type is only child of player/AI
        helicopter = transform.GetChild(0).gameObject;

        if (helicopter.CompareTag("tandem"))
        {
            FlightController = helicopter.GetComponent<TandemFlightController>();
        }
        else if (helicopter.CompareTag("conventional"))
        {
            FlightController = helicopter.GetComponent<ConventionalFlightController>();
        }
        else if (helicopter.CompareTag("coax"))
        {
            FlightController = helicopter.GetComponent<CoaxFlightController>();
        }
        else if (helicopter.CompareTag("quad"))
        {
            FlightController = helicopter.GetComponent<QuadFlightController>();
        }
        else if (helicopter.CompareTag("sidebyside"))
        {
            FlightController = helicopter.GetComponent<SideBySideFlightController>();
        }

        HoverControl = new HoverControlComponent(FlightController);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (HoverMode)
        {
            HoverControl.Update();
        }
        
        ProcessLift();
        ProcessPitch();
        ProcessYaw();
        ProcessRoll();
        ProcessThrust();
    }

    private void ToggleHoverMode(InputAction.CallbackContext context)
    {
        HoverMode = !HoverMode;
        HoverControl.TargetAltitude = helicopter.transform.position.y;
        HoverControl.TargetHeading = helicopter.transform.forward;
    }

    private void ProcessLift(){
        float pow = LiftInput.ReadValue<float>();
        if (pow != 0)
        {
            HoverControl.TargetAltitude = helicopter.transform.position.y;
        }

        FlightController.ApplyLift(pow);

    }

    private void ProcessPitch(){
        float pitch = PitchInput.ReadValue<float>();

        FlightController.ApplyPitch(pitch);
    }

    private void ProcessRoll(){
        float roll = RollInput.ReadValue<float>();

        FlightController.ApplyRoll(roll);
    }

    private void ProcessYaw(){
        float yaw = YawInput.ReadValue<float>();
        if (yaw != 0)
        {
            HoverControl.TargetHeading = helicopter.transform.forward;
        }
        FlightController.ApplyYaw(yaw);
    }

    private void ProcessThrust(){
        float thrust = ThrustInput.ReadValue<float>();

        FlightController.ApplyThrust(thrust);
    }
}
