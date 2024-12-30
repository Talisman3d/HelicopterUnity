using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Class pertains to rotors that can apply moments. Simulates helicopter cyclic
public class Cyclic : MonoBehaviour
{
    Rigidbody rb; // Rigid Body of rotor

    Vector3 torque; // torque

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Set rigid body of rotor
    }

    public void applyTorque(float torqueInput, Vector3 torque, float multiplier)
    {
        // Apply rotor torque to rotor
        rb.AddRelativeTorque(torqueInput * torque* multiplier * Time.fixedDeltaTime * 10);
    }
}
