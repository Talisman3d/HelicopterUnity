using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Class pertains to rotors that can only apply thrust
public class Collective : MonoBehaviour
{

    Rigidbody rb; // Rigid body of rotor
    float powVal; // How much thrust is applied
    Rigidbody parentRB; // Helicopter's rigid body
    
    public float antiTorque; // Simulating reaction to rotor torque
    float powerInput; // How much torque is input from -1 to 1
    Vector3 powerDirection = Vector3.up; // Rotor can only thrust in it's y direction

    void Start() 
    { 
        rb = GetComponent<Rigidbody>(); // get rigid body of rotor
        parentRB = transform.parent.gameObject.GetComponent<Rigidbody>(); // get rigid body of parent
    } 

    private void FixedUpdate() {
        powVal=0;   // Reset power/thrust to 0, for debugging visualization purposes
    }

    private void ProcessAntiTorque(float antiTorque, float powVal){
        // Apply reaction of rotor torque to helicopter rigid body
        parentRB.AddRelativeTorque(transform.up* Time.fixedDeltaTime*powVal*antiTorque);
    }

    public void applyPow(float powerGiven, float PowerMult)
    {
        // Apply thrust to rotor
        powerInput=powerGiven; // Set to global variable for access elsewhere
        powVal = powerInput * PowerMult * Time.fixedDeltaTime * 10; //  powerInput is -1 to 1, powerMult is cusomtizable in Inspector
        rb.AddRelativeForce(powerDirection* powVal); // Apply thrust to rotor rigid body
        ProcessAntiTorque(antiTorque, powVal); // Handle reaction of thrust on helicopter parent rigid body
    }

    public float returnPow(){
        // Used to access total thrust elsewhere
        return powVal;
    }

    public float returnPowInput(){
        // Used to access powerInput (-1 to 1) elsewhere
        return powerInput;
    }
}
