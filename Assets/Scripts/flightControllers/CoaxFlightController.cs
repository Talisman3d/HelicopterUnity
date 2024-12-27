using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Translates input to Coax Flight Output
public class CoaxFlightController : FlightController
{
    [SerializeField] GameObject UpperRotor;
    [SerializeField] GameObject LowerRotor;
    [SerializeField] GameObject PropThruster;
    [SerializeField] float LiftSensitivity = 150f;
    [SerializeField] float PitchSensitivity = 40f;
    [SerializeField] float YawSensitivity = 8f;
    [SerializeField] float RollSensitivity = 40f;
    [SerializeField] float ThrustSensitivity = 40f;


    float rotorThrust;
    Vector3 rotorRotation;
    float antiTorqueVal;

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
            UpperRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
            LowerRotor.GetComponent<Collective>().applyPow(pow, LiftSensitivity);
        }
    }

   public override void ApplyPitch(float pitch){
        if (pitch!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, PitchSensitivity);
            LowerRotor.GetComponent<Cyclic>().applyTorque(pitch, Vector3.left, PitchSensitivity);
        }
    }

    public override void ApplyRoll(float roll){
        if (roll!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, RollSensitivity);
            LowerRotor.GetComponent<Cyclic>().applyTorque(roll, Vector3.back, RollSensitivity);
        }
    }

    public override void ApplyYaw(float yaw){ 
        
        if (yaw!=0)
        {
            UpperRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
            LowerRotor.GetComponent<Cyclic>().applyTorque(yaw, Vector3.up, YawSensitivity);
        }
    }

    public override void ApplyThrust(float thrust){
        
        if (thrust!=0)
        {
            PropThruster.GetComponent<Collective>().applyPow(thrust, ThrustSensitivity);
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