using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

// Button press resets helicopter object(s) and landing pad location
public class ResetPosition : MonoBehaviour
{
        [SerializeField] InputAction resetPos; // Button input to reset player helicopter
        [SerializeField] GameObject landingPad; // Landing Pad object
        [SerializeField] GameObject[] helicopterObj; // Array of other helicopter objects
        Vector3[] startPos; // Array of helicopter starting positions
        quaternion[] startRot; // Array of helicopter starting rotations
        Vector3 startingPadLoc; // Starting Pad location
        Rigidbody[] rb; // Array of helicopter rigid bodies

        GameObject flightController; // Flight Controller game object

    void OnEnable() {
        resetPos.Enable();
        resetPos.performed += resetLevel; // When the reset button is pressed, do this function
    }

    void Start()
    {
        landingPad = instantiateLandingPad(); // Reset Landing pad

        helicopterObj[0] = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject; // Automatically set player helicopter

        startPos = new Vector3[helicopterObj.Length]; 
        startRot = new quaternion[helicopterObj.Length];
        rb = new Rigidbody[helicopterObj.Length];

        for(int i=0;i<helicopterObj.Length;i++){
            startPos[i]=helicopterObj[i].transform.position;
            startRot[i]=helicopterObj[i].transform.rotation;

            rb[i]=helicopterObj[i].GetComponent<Rigidbody>();
        }
    }

    void resetLevel(InputAction.CallbackContext context){

        // Reset Helicopter objects
        for(int i=0;i<helicopterObj.Length;i++){
            helicopterObj[i].transform.position=startPos[i];
            helicopterObj[i].transform.rotation=startRot[i];
            rb[i].linearVelocity  = Vector3.zero;
            rb[i].angularVelocity = Vector3.zero;
        }

        // Reset Helicopter objects' rotors momentum
        ResetRotorInertia();

        // Respawn landing pad
        Destroy(landingPad);
        landingPad = instantiateLandingPad();
    }

    GameObject instantiateLandingPad(){
        startingPadLoc=new Vector3(UnityEngine.Random.Range(-20f,20f),UnityEngine.Random.Range(1f,17f),UnityEngine.Random.Range(0f,20f));
        GameObject lp = Instantiate(landingPad, startingPadLoc,  
                                                     transform.rotation);
        return lp;
    }

    void ResetRotorInertia(){
        GameObject[] rotorGCs = GameObject.FindGameObjectsWithTag("rotors");
        for(int i=0;i<rotorGCs.Length;i++){
            rotorGCs[i].GetComponent<Rigidbody>().linearVelocity  = Vector3.zero;
            rotorGCs[i].GetComponent<Rigidbody>().angularVelocity  = Vector3.zero;
        }
    }

}
