using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Coax Flight Output
public class CoaxFlightController : FlightController
{
    [Header("Child Rotor Objects")]
    [SerializeField] GameObject UpperRotor;
    [SerializeField] GameObject LowerRotor;
    [SerializeField] GameObject PropThruster;

    [Header("Rotor Maximum Thrust and Torque")]
    [SerializeField] float liftMax = 150f;
    [SerializeField] float pitchMax = 40f;
    [SerializeField] float yawMax = 8f;
    [SerializeField] float rollMax = 40f;
    [SerializeField] float thrustMax = 40f;

    // References to rotors and helicopter rigid body
    GameObject[] rotorGCs;
    Rigidbody rb;

    void Start()
    {
        rotorGCs = new GameObject[3];
        rotorGCs[0]=UpperRotor;
        rotorGCs[1]=LowerRotor;
        rotorGCs[2]=PropThruster;
        rb = GetComponent<Rigidbody>();
    }
    public override void ApplyLift(float pow){
        if (pow!=0)
        {
            UpperRotor.GetComponent<Collective>().applyPow(pow, liftMax);
            LowerRotor.GetComponent<Collective>().applyPow(pow, liftMax);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, pitchMax);
            LowerRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, pitchMax);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, rollMax);
            LowerRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, rollMax);
        }
    }

    public override void ApplyYaw(float yaw){ 
        
        if (yaw!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, yawMax);
            LowerRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, yawMax);
        }
    }

    public override void ApplyThrust(float thrust){
        
        if (thrust!=0)
        {
            PropThruster.GetComponent<Collective>().applyPow(thrust, thrustMax);
        }
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