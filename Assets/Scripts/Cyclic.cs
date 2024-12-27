using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cyclic : MonoBehaviour
{
    Rigidbody rb;

    Vector3 torque;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void applyTorque(float torqueInput, Vector3 torque, float multiplier)
    {
        rb.AddRelativeTorque(torqueInput * torque* multiplier * Time.fixedDeltaTime * 10);
    }



}
