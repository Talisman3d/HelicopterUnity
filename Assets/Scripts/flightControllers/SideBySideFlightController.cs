using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Side by Side Flight Output
public class SideBySideFlightController : FlightController
{
    [Header("Child Rotor Objects")]
    [SerializeField] GameObject LeftPropRotor;
    [SerializeField] GameObject RightPropRotor;
    

    [Header("Rotor Maximum Thrust and Torque")]
    [SerializeField] float LiftSensitivity = 150f;
    [SerializeField] float PitchSensitivity = 40f;
    [SerializeField] float YawSensitivity = 8f;
    [SerializeField] float RollSensitivity = 40f;

    // References to rotors and helicopter rigid body
    GameObject[] rotorGCs;
    Rigidbody rb;

    void Start()
    {
        rotorGCs = new GameObject[2];
        rotorGCs[0]=LeftPropRotor;
        rotorGCs[1]=RightPropRotor;
        rb = GetComponent<Rigidbody>();
    }

    public override void ApplyLift(float pow){
        if (pow!=0)
        {
            LeftPropRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
            RightPropRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            LeftPropRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, PitchSensitivity);
            RightPropRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, PitchSensitivity);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            LeftPropRotor.GetComponent<Collective>().applyPow(roll, RollSensitivity);
            RightPropRotor.GetComponent<Collective>().applyPow(-roll, RollSensitivity);
        }
    }
    public override void ApplyYaw(float yaw){
        if (yaw!=0)
        {
            LeftPropRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
            RightPropRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
        }
    }

    public override void ApplyThrust(float thrustInput)
    {
        
    }

    public void ResetRotorInertia(){
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        for(int i=0;i<rotorGCs.Length;i++){
            rotorGCs[i].GetComponent<Rigidbody>().linearVelocity  = Vector3.zero;
            rotorGCs[i].GetComponent<Rigidbody>().angularVelocity  = Vector3.zero;
        }
    }
}