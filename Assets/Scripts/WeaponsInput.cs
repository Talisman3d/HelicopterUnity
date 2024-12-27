using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


public class WeaponsInput : MonoBehaviour
{
    [SerializeField] InputAction gun;
    [SerializeField] InputAction rockets;
    GameObject heli;
    GameObject weaponMaster;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnEnable() {
        gun.Enable();
        rockets.Enable();
    }
    void Start()
    {
        heli = transform.GetChild(0).gameObject;
        weaponMaster = heli.transform.Find("Weapons").gameObject;

        rockets.performed += fireRockets;
    }

    void FixedUpdate(){
        if (gun.IsPressed()){
            weaponMaster.GetComponent<Weapons>().startGunSound();
            weaponMaster.GetComponent<Weapons>().fireGun();
            Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        }
        else{
            weaponMaster.GetComponent<Weapons>().stopGunSound();
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    private void fireRockets(InputAction.CallbackContext context){
        
        //weaponMaster.GetComponent<Weapons>().fireRockets();
        weaponMaster.GetComponent<Weapons>().StartCoroutine(weaponMaster.GetComponent<Weapons>().rocketVolley());
    }
}
