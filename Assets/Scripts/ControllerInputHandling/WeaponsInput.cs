using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Player input for weapons
public class WeaponsInput : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputAction gunInput; // Eventually I want this to be modular
    [SerializeField] InputAction rocketsInput; // Eventually I want this to be modular
    [SerializeField] InputAction missileInput; // Eventually I want this to be modular
    [SerializeField] InputAction harpoonInput; // Eventually I want this to be modular
    [SerializeField] InputAction droneInput;

    // References to other objects
    GameObject heli; // Player helicopter
    GameObject weaponMaster; // Weapons manager object
    WeaponController weaponScript; // script that handles all weapon behaviors and prefabs

    private void OnEnable() {
        gunInput.Enable();
        rocketsInput.Enable();
        missileInput.Enable();
        harpoonInput.Enable();
        droneInput.Enable();
    }
    void Start()
    {
        heli = transform.GetChild(0).gameObject; // Grab player helicopter
        weaponMaster = heli.transform.Find("Weapons").gameObject; // Grab WeaponsController script
        weaponScript= weaponMaster.GetComponent<WeaponController>();

        rocketsInput.performed += fireRockets; // Fire rockets when rocket button pressed
        missileInput.performed += fireMissile; // Fire rockets when rocket button pressed
        harpoonInput.performed += harpoonAway;
        droneInput.performed += launchQuad;
    }

    void FixedUpdate(){
        if (gunInput.IsPressed()){
            weaponScript.startGunSound();
            weaponScript.fireGun();
            Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        }
        else{
            weaponScript.stopGunSound();
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    private void fireRockets(InputAction.CallbackContext context){
        weaponScript.StartCoroutine(weaponMaster.GetComponent<WeaponController>().rocketVolley());
    }

    private void fireMissile(InputAction.CallbackContext context){
        weaponScript.StartCoroutine(weaponMaster.GetComponent<WeaponController>().missileVolley());
    }

    private void launchQuad(InputAction.CallbackContext context){
        weaponScript.launchQuad();
    }

    private void harpoonAway(InputAction.CallbackContext context){
        weaponScript.fireGrapplingHook();
    }
}
