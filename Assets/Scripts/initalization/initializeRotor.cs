using UnityEngine;

// Class simply sets parent of rotor so I don't have to in inspector
public class initializeRotor : MonoBehaviour
{
    GameObject helicopter;
    FixedJoint fj;

    void Start()
    {
        helicopter = transform.parent.gameObject;
        fj = GetComponent<FixedJoint>();
        fj.connectedBody = helicopter.GetComponent<Rigidbody>();
    }
}
