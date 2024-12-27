using UnityEngine;

// I feel as though I'm insulting your intelligence by adding comments in this one
public class SpinRotors : MonoBehaviour
{
    [SerializeField] float rotorSpeed = 25f;
    void Update()
    {
         gameObject.transform.Rotate(Vector3.up*rotorSpeed*Time.deltaTime);
    }
}
