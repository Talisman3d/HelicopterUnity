using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Conventional Flight Output
public class ConventionalFlightController : FlightController
{
    [Header("Child Rotor Objects")]
    [SerializeField] GameObject MainRotor;
    [SerializeField] GameObject TailRotor;

    [Header("Rotor Maximum Thrusts")]
    [SerializeField] float liftMax = 150f;
    [SerializeField] float pitchMax = 40f;
    [SerializeField] float yawMax = 8f;
    [SerializeField] float rollMax = 40f;

    // References to rotors and helicopter rotor body
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
            MainRotor.GetComponent<Collective>().applyPow(pow, liftMax);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            MainRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, pitchMax);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            MainRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, rollMax);
        }
    }

    public override void ApplyYaw(float yaw){
        if (yaw!=0)
        {
            TailRotor.GetComponent<Collective>().applyPow(yaw, yawMax);
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
