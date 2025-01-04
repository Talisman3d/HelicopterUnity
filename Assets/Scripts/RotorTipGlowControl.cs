using System;
using UnityEngine;

// Quick and easy script to handle rotor tip glowy effect
public class RotorTipGlowControl : MonoBehaviour
{    
    [Header("All Settings")]
    [SerializeField] Boolean glowIsOnBool = true;

    [Header("Main Rotor")]
    [SerializeField] GameObject[] mainRotorTipGlowObjects;
    [SerializeField] float mainRotorTrailDuration = 0.5f;
    [SerializeField] float mainRotorTrailThickness = 0.1f;
    TrailRenderer[] mrTrailRenderer;


    [Header("TailRotor")]
    [SerializeField] GameObject[] tailRotorTipGlowObjects;
    [SerializeField] float tailRotorTrailDuration = 0.5f;
    [SerializeField] float tailRotorTrailThickness = 0.05f;
    TrailRenderer[] trTrailRenderer;

    void Start()
    {
        mrTrailRenderer = new TrailRenderer[mainRotorTipGlowObjects.Length];
        trTrailRenderer = new TrailRenderer[tailRotorTipGlowObjects.Length];
        
        for(int i=0;i<mainRotorTipGlowObjects.Length;i++){
            mrTrailRenderer[i]=mainRotorTipGlowObjects[i].GetComponent<TrailRenderer>();
        }

        for(int j=0;j<tailRotorTipGlowObjects.Length;j++){
            trTrailRenderer[j]=tailRotorTipGlowObjects[j].GetComponent<TrailRenderer>();
        }

        if (glowIsOnBool){
            foreach (TrailRenderer rndr in mrTrailRenderer){
                rndr.enabled=true;
                rndr.time=mainRotorTrailDuration;
                rndr.startWidth=mainRotorTrailThickness;
            }

            foreach (TrailRenderer rndr in trTrailRenderer){
                rndr.enabled=true;
                rndr.time=tailRotorTrailDuration;
                rndr.startWidth=tailRotorTrailThickness;
            }
        }
        else{
            foreach (TrailRenderer rndr in mrTrailRenderer){
                rndr.enabled=false;
            }

            foreach (TrailRenderer rndr in trTrailRenderer){
                rndr.enabled=false;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Seems like a waste of processing, but maybe I'll need to turn it on when it gets dark
        if (glowIsOnBool){
            foreach (TrailRenderer rndr in mrTrailRenderer){
                rndr.enabled=true;
                rndr.time=mainRotorTrailDuration;
                rndr.startWidth=mainRotorTrailThickness;
            }

            foreach (TrailRenderer rndr in trTrailRenderer){
                rndr.enabled=true;
                rndr.time=tailRotorTrailDuration;
                rndr.startWidth=tailRotorTrailThickness;
            }
        }
        else{
            foreach (TrailRenderer rndr in mrTrailRenderer){
                rndr.enabled=false;
            }

            foreach (TrailRenderer rndr in trTrailRenderer){
                rndr.enabled=false;
            }
        }

    }
}
