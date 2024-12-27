using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SideBySideFlightController : FlightController
{
 [SerializeField] GameObject LeftPropRotor;
    [SerializeField] GameObject RightPropRotor;
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
        rotorGCs[0]=LeftPropRotor;
        rotorGCs[1]=RightPropRotor;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ProcessAntiTorque();
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
    private void ProcessAntiTorque(){
        for(int i=0;i<rotorGCs.Length;i++){
            rotorThrust=rotorGCs[i].GetComponent<Collective>().returnPow();
            rotorRotation=rotorGCs[i].transform.up;
            antiTorqueVal = rotorGCs[i].GetComponent<Collective>().antiTorque;
            rb.AddRelativeTorque(rotorRotation * Time.fixedDeltaTime * rotorThrust * antiTorqueVal);
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