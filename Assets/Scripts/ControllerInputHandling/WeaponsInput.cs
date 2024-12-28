using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Player input for weapons
public class WeaponsInput : MonoBehaviour
{
    [SerializeField] InputAction gunInput; // Eventually I want this to be modular
    [SerializeField] InputAction rocketsInput; // Eventually I want this to be modular
    [SerializeField] InputAction missileInput; // Eventually I want this to be modular
    GameObject heli; // Player helicopter
    GameObject weaponMaster; // WeaponsController

    private void OnEnable() {
        gunInput.Enable();
        rocketsInput.Enable();
        missileInput.Enable();
    }
    void Start()
    {
        heli = transform.GetChild(0).gameObject; // Grab player helicopter
        weaponMaster = heli.transform.Find("Weapons").gameObject; // Grab WeaponsController script

        rocketsInput.performed += fireRockets; // Fire rockets when rocket button pressed
        missileInput.performed += fireMissile; // Fire rockets when rocket button pressed
    }

    void FixedUpdate(){
        if (gunInput.IsPressed()){
            weaponMaster.GetComponent<WeaponController>().startGunSound();
            weaponMaster.GetComponent<WeaponController>().fireGun();
            Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        }
        else{
            weaponMaster.GetComponent<WeaponController>().stopGunSound();
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    private void fireRockets(InputAction.CallbackContext context){
        weaponMaster.GetComponent<WeaponController>().StartCoroutine(weaponMaster.GetComponent<WeaponController>().rocketVolley());
    }

    private void fireMissile(InputAction.CallbackContext context){
        weaponMaster.GetComponent<WeaponController>().StartCoroutine(weaponMaster.GetComponent<WeaponController>().missileVolley());
    }
}
