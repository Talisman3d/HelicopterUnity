using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RandomPad : MonoBehaviour
{

    [SerializeField] GameObject landingPad;
    
    [SerializeField] InputAction resetPos;
    
    Vector3 startingLoc;

    void OnEnable() {
        resetPos.performed += randomizeLandingPadLoc;
        resetPos.Enable();
    }
    void Start()
    {
        
    }

    void randomizeLandingPadLoc(InputAction.CallbackContext context){
        startingLoc=new Vector3(Random.Range(-20f,20f),Random.Range(1f,17f),Random.Range(0f,20f));
        GameObject lp = Instantiate(landingPad, startingLoc,  
                                                     transform.rotation);
    }
}
