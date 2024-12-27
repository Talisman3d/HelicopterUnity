using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
public class Collective : MonoBehaviour
{

    Rigidbody rb;
    float powVal;

    Rigidbody parentRB;
    

    public float antiTorque;
    float powerInput;

    Vector3 powerDirection = Vector3.up;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    { 
        rb = GetComponent<Rigidbody>();
        parentRB = transform.parent.gameObject.GetComponent<Rigidbody>();
    } 

    private void FixedUpdate() {
        powVal=0;
    }

    private void ProcessAntiTorque(float antiTorque, float powVal){
        parentRB.AddRelativeTorque(transform.up* Time.fixedDeltaTime*powVal*antiTorque);
    }

    public void applyPow(float powerGiven, float PowerMult)
    {
        powerInput=powerGiven;
        powVal = powerInput * PowerMult * Time.fixedDeltaTime * 10;
        
        rb.AddRelativeForce(powerDirection* powVal);

        ProcessAntiTorque(antiTorque, powVal);
    }

    public float returnPow(){
        
        return powVal;
    }

    public float returnPowInput(){
        return powerInput;
    }
}
