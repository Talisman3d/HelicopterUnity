using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to quad flight output
public class QuadFlightController : FlightController
{
    [Header("Child Rotor Objects")]
    [SerializeField] GameObject Rotor1;
    [SerializeField] GameObject Rotor2;
    [SerializeField] GameObject Rotor3;
    [SerializeField] GameObject Rotor4;

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
        rotorGCs = new GameObject[4];
        rotorGCs[0]=Rotor1;
        rotorGCs[1]=Rotor2;
        rotorGCs[2]=Rotor3;
        rotorGCs[3]=Rotor4;
        rb = GetComponent<Rigidbody>();
    }
    public override void ApplyLift(float pow){
        if (pow!=0)
        {
            Rotor1.GetComponent<Collective>().applyPow(pow,  liftMax);
            Rotor2.GetComponent<Collective>().applyPow(pow, liftMax);
            Rotor3.GetComponent<Collective>().applyPow(pow, liftMax);
            Rotor4.GetComponent<Collective>().applyPow(pow, liftMax);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            Rotor1.GetComponent<Collective>().applyPow(pitch, pitchMax);
            Rotor2.GetComponent<Collective>().applyPow(pitch, pitchMax);
            Rotor3.GetComponent<Collective>().applyPow(-pitch, pitchMax);
            Rotor4.GetComponent<Collective>().applyPow(-pitch, pitchMax);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            Rotor1.GetComponent<Collective>().applyPow(roll, rollMax);
            Rotor2.GetComponent<Collective>().applyPow(-roll, rollMax);
            Rotor3.GetComponent<Collective>().applyPow(-roll, rollMax);
            Rotor4.GetComponent<Collective>().applyPow(roll, rollMax);
        }
    }

    // WIP, not behaving as expected.
    // Should just need to apply power up on 1 and 3, down on 2 and 4 and reverse to yaw
    public override void ApplyYaw(float yaw){
        if (yaw>0)
        {
            //Rotor1.GetComponent<Collective>().applyPow(Vector3.up, YawSensitivity);
            Rotor2.GetComponent<Collective>().applyPow(yaw, yawMax);
            //Rotor3.GetComponent<Collective>().applyPow(Vector3.up, YawSensitivity);
            //Rotor4.GetComponent<Collective>().applyPow(yaw, Vector3.down, YawSensitivity);
        }
        else if (yaw<0){
            Rotor1.GetComponent<Collective>().applyPow(-yaw, yawMax);
            // Rotor2.GetComponent<Collective>().applyPow(yaw, Vector3.up, YawSensitivity);
            //Rotor3.GetComponent<Collective>().applyPow(-yaw, Vector3.up, YawSensitivity);
            // Rotor4.GetComponent<Collective>().applyPow(yaw, Vector3.down, YawSensitivity);
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