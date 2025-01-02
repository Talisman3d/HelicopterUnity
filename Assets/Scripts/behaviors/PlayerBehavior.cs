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
    public bool AutoPilotMode { get; protected set; }
    GameObject helicopter;

    [SerializeField] InputAction HoverModeInput;
    [SerializeField] InputAction AutoPilotInput;

    private void OnEnable() {
        HoverModeInput.Enable();    
        HoverModeInput.performed += ToggleHoverMode;    

        AutoPilotInput.Enable();  
        AutoPilotInput.performed += ToggleAutoPilot;
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
    }

    private void ToggleAutoPilot(InputAction.CallbackContext context){
        AutoPilotMode = !AutoPilotMode;
    }

    private void Update() {

        if(HoverMode||AutoPilotMode){
            autoPilotScript.enabled=true;
            playerInputScript.enabled=false;
        }
        else{
            autoPilotScript.enabled=false;
            playerInputScript.enabled=true;
        }
    }

}
