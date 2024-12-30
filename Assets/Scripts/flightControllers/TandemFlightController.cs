using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Tandem Flight Output
public class TandemFlightController : FlightController
{
    [Header("Child Rotor Objects")]
    [SerializeField] GameObject FrontRotor;
    [SerializeField] GameObject RearRotor;

    [Header("Rotor Maximum Thrust and Torque")]
    [SerializeField] float liftMax = 150f;
    [SerializeField] float pitchMax = 40f;
    [SerializeField] float yawMax = 8f;
    [SerializeField] float rollMax = 40f;

    // References to rotors and helicopter rigid body
    GameObject[] rotorGCs;
    Rigidbody rb;

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
            FrontRotor.GetComponent<Collective>().applyPow(pow, liftMax);
            RearRotor.GetComponent<Collective>().applyPow(pow, liftMax);
        }
    }

    public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            FrontRotor.GetComponent<Collective>().applyPow(pitch, pitchMax);
            RearRotor.GetComponent<Collective>().applyPow(-pitch, pitchMax);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            FrontRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, rollMax);
            RearRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, rollMax);
        }
    }

    public override void ApplyYaw(float yaw){
        if (yaw!=0)
        {
            FrontRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, yawMax);
            RearRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, yawMax);
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