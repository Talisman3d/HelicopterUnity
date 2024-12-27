using UnityEngine;

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
