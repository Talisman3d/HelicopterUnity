using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Spawns a random landing pad for sandbox level
public class RandomPad : MonoBehaviour
{

    [SerializeField] GameObject landingPad; // Landing pad prefab
    [SerializeField] InputAction resetPos; // Button input to reset level
    
    Vector3 startingLoc;

    void OnEnable() {
        resetPos.performed += randomizeLandingPadLoc;
        resetPos.Enable();
    }

    void randomizeLandingPadLoc(InputAction.CallbackContext context){
        startingLoc=new Vector3(Random.Range(-20f,20f),Random.Range(1f,17f),Random.Range(0f,20f));
        GameObject lp = Instantiate(landingPad, startingLoc,  
                                                     transform.rotation);
    }
}
