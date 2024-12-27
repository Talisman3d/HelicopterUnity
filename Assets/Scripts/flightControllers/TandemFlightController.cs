using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Tandem Flight Output
public class TandemFlightController : FlightController
{
    [SerializeField] GameObject FrontRotor;
    [SerializeField] GameObject RearRotor;
    [SerializeField] float LiftSensitivity = 150f;
    [SerializeField] float PitchSensitivity = 40f;
    [SerializeField] float YawSensitivity = 8f;
    [SerializeField] float RollSensitivity = 40f;

    float rotorThrust;
    Vector3 rotorRotation;
    float antiTorqueVal;

    GameObject[] rotorGCs;

    Rigidbody rb;

    float pow;
    float pitch;
    float roll;
    float yaw;

    void Start()
    {
        rotorGCs = new GameObject[2];
        rotorGCs[0]=FrontRotor;
        rotorGCs[1]=RearRotor;
        rb = GetComponent<Rigidbody>();
    }
    public override void ApplyLift(float pow){
        if (pow!=0)
        {
            FrontRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
            RearRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
        }
    }

    public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            FrontRotor.GetComponent<Collective>().applyPow(pitch, PitchSensitivity);
            RearRotor.GetComponent<Collective>().applyPow(-pitch, PitchSensitivity);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            FrontRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, RollSensitivity);
            RearRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, RollSensitivity);
        }
    }

    public override void ApplyYaw(float yaw){
        if (yaw!=0)
        {
            FrontRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
            RearRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
        }
    }

    public override void ApplyThrust(float thrust){

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