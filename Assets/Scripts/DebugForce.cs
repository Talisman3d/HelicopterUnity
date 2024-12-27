using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DebugForce : MonoBehaviour
{
    GameObject forceArrow;

    [SerializeField] Boolean debugMode =  true; 
    float pow;
    Vector3 scaleVec;

    Vector3 originalScale;



    void Start()
    {
        forceArrow= transform.Find("ForceArrow").gameObject;
        forceArrow.SetActive(false);
        
        if (debugMode){
            forceArrow.SetActive(true);
            originalScale = forceArrow.transform.localScale;         
            forceArrow.transform.position=transform.position;
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        if(debugMode){
            pow = GetComponent<Collective>().returnPowInput();
            
            scaleVec = new Vector3(originalScale.x,pow,originalScale.z);
            forceArrow.transform.localScale=scaleVec;
        }
    }
}
