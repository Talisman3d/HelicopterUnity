using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Conventional Flight Output
public class ConventionalFlightController : FlightController
{
    [SerializeField] GameObject MainRotor;
    [SerializeField] GameObject TailRotor;
    [SerializeField] float LiftSensitivity = 150f;
    [SerializeField] float PitchSensitivity = 40f;
    [SerializeField] float YawSensitivity = 8f;
    [SerializeField] float RollSensitivity = 40f;

    float rotorThrust;
    Vector3 rotorRotation;
    float antiTorqueVal;

    GameObject[] rotorGCs;

    Rigidbody rb;

    void Start()
    {
        rotorGCs = new GameObject[2];
        rotorGCs[0]=MainRotor;
        rotorGCs[1]=TailRotor;
        rb = GetComponent<Rigidbody>();
    }
    
    public override void ApplyLift(float pow){
        if (pow!=0)
        {
            MainRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            MainRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, PitchSensitivity);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            MainRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, RollSensitivity);
        }
    }

    public override void ApplyYaw(float yaw){
        if (yaw!=0)
        {
            TailRotor.GetComponent<Collective>().applyPow(yaw, YawSensitivity);
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
