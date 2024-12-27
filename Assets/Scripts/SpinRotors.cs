using UnityEngine;

public class SpinRotors : MonoBehaviour
{
    [SerializeField] float rotorSpeed = 25f;
    void Update()
    {
         gameObject.transform.Rotate(Vector3.up*rotorSpeed*Time.deltaTime);
    }
}
