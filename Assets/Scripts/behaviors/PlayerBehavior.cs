using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Handle player behavior via enabling/disabling monoscripts
// Currently switches between allowing player input and turing on autopilot
// Same will be done for AI
public class PlayerBehavior : MonoBehaviour
{

    // References to other scripts
    PlayerInputController playerInputScript;
    AutoPilot autoPilotScript;   

    // Reference to Helicopter object
    public bool HoverMode { get; protected set; }
    public bool FlyToTargetMode { get; protected set; }
    public bool FlyToTargetSet { get; protected set; }

    GameObject helicopter;
    GameObject target;

    [SerializeField] InputAction HoverModeInput;
    [SerializeField] InputAction FlyToTargetInput;

    private void OnEnable() {
        HoverModeInput.Enable();    
        HoverModeInput.performed += ToggleHoverMode;    

        FlyToTargetInput.Enable();  
        FlyToTargetInput.performed += ToggleFlyToTargetMode;

        autoPilotScript=GetComponent<AutoPilot>();
        target = GameObject.Find("TrackingSphere");
    }

    private void Start()
    {
        // Get references to the scripts
        playerInputScript = GetComponent<PlayerInputController>();
        autoPilotScript = GetComponent<AutoPilot>();

        helicopter = transform.GetChild(0).gameObject;
    }

    private void ToggleHoverMode(InputAction.CallbackContext context)
    {
        HoverMode = !HoverMode;
        if(HoverMode){
            autoPilotScript.isAutoPilotEnabled=true;
            autoPilotScript.HoverInPlace();
        }
        else{
            autoPilotScript.isAutoPilotEnabled=false;
        }
    }

    private void ToggleFlyToTargetMode(InputAction.CallbackContext context){
        if(HoverMode){
            FlyToTargetMode = !FlyToTargetMode;
            if(FlyToTargetMode){
                autoPilotScript.MoveToTarget(target);
            }
            else{
                autoPilotScript.HoverInPlace();
            }
        }

    }

}
