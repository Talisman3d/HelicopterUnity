using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Class visualizes rotor collective force from -1 to 1
public class DebugForce : MonoBehaviour
{
    GameObject forceArrow;  // Visualization, ForceArrow prefab

    [SerializeField] Boolean debugMode =  true; // Toggle currently per rotor, will eventually set a global to toggle all
    float pow; // Power/thrust
    Vector3 scaleVec; // Holds the current scale of ForceArrow

    Vector3 originalScale; // Captures the original scale of ForceArrow prefab

    void Start()
    {
        // Assuming Force Arrow is a child of Rotor object (it is as a prefab)
        forceArrow= transform.Find("ForceArrow").gameObject;
        // Disabled by default upon game start (so I can see it in Scene editor)
        forceArrow.SetActive(false);
        
        if (debugMode){
            // Initialize Force Arrow if desired
            forceArrow.SetActive(true);
            originalScale = forceArrow.transform.localScale;         
            forceArrow.transform.position=transform.position;
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        if(debugMode){
            pow = GetComponent<Collective>().returnPowInput(); // Grab the PowerInput (-1 to 1). Can be player controller driven or AI inputs
            scaleVec = new Vector3(originalScale.x,pow,originalScale.z); // Scale only the Y value
            forceArrow.transform.localScale=scaleVec; // Apply scale to ForceArrow object
        }
    }
}
