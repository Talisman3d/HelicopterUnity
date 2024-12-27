using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] InputAction resetPos;
        [SerializeField] GameObject landingPad;
        [SerializeField] GameObject[] helicopterObj;
        Vector3[] startPos;
        quaternion[] startRot;
        Vector3 startingPadLoc;
        Rigidbody[] rb;

        GameObject flightController;

    void OnEnable() {
        resetPos.performed += resetLevel;
        resetPos.Enable();
    }

    void Start()
    {

        landingPad = instantiateLandingPad();

        helicopterObj[0] = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;

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
        for(int i=0;i<helicopterObj.Length;i++){
            helicopterObj[i].transform.position=startPos[i];
            helicopterObj[i].transform.rotation=startRot[i];
            rb[i].linearVelocity  = Vector3.zero;
            rb[i].angularVelocity = Vector3.zero;
        }

        ResetRotorInertia();

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
