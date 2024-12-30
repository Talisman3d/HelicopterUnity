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

    // References to objects and classes
    GameObject helicopter;
    FlightController flightController;

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

    private void ProcessLift(){
        float pow = LiftInput.ReadValue<float>();

        flightController.ApplyLift(pow);

    }

    private void ProcessPitch(){
        float pitch = PitchInput.ReadValue<float>();

        flightController.ApplyPitch(pitch);
    }

    private void ProcessRoll(){
        float roll = RollInput.ReadValue<float>();

        flightController.ApplyRoll(roll);
    }

    private void ProcessYaw(){
        float yaw = YawInput.ReadValue<float>();

        flightController.ApplyYaw(yaw);
    }

    private void ProcessThrust(){
        float thrust = ThrustInput.ReadValue<float>();

        flightController.ApplyThrust(thrust);
    }
}
