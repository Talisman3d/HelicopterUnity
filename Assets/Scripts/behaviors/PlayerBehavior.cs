using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Handle player behavior via enabling/disabling monoscripts
// Currently switches between allowing player input and turing on autopilot
// Same will be done for AI
public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] InputAction SwitchModes;

    MoveToTarget moveToTargetScript;
    PlayerInputController playerInputScript;

    Boolean targetSet=false;
    Boolean isAutopilotActive = false;

    GameObject helicopter;

    private void OnEnable() {
        SwitchModes.Enable();        
    }

    private void Start()
    {
        // Get references to the scripts
        moveToTargetScript = GetComponent<MoveToTarget>();
        playerInputScript = GetComponent<PlayerInputController>();

        helicopter = transform.GetChild(0).gameObject;
        
        // Register the performed event
        SwitchModes.performed += toggleControl;
    }

    private void toggleControl(InputAction.CallbackContext context){
        if (isAutopilotActive)
        {
            EnablePlayerInput();  // Switch to manual player control
        }
        else
        {
            EnableMoveToTarget(); // Switch to autopilot control
        }
    }
    

    private void EnablePlayerInput(){
        targetSet=false;
        isAutopilotActive = false;
        moveToTargetScript.enabled = false;  // Enable autopilot
        playerInputScript.enabled = true;  // Disable player input

    }

    private void EnableMoveToTarget(){
        isAutopilotActive = true;
        if(!targetSet){
            moveToTargetScript.setTarget(helicopter.transform.position);
            targetSet=true;
        }
        moveToTargetScript.enabled = true;  // Enable autopilot
        playerInputScript.enabled = false;  // Disable player input
        
    }
}
