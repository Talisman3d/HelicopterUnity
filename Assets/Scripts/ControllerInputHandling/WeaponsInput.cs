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

    public LayerMask collisionLayer;

    public GameObject crossHairsPrefab;   // Reference to the dot prefab
    private GameObject crossHairs;        // Instance of the dot

    public Camera mainCamera;         // Reference to the main camera

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

        // Instantiate Crosshairs in front of helicopter
        crossHairs = Instantiate(crossHairsPrefab, Vector3.zero, Quaternion.identity);
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

        showWeaponsReticle();
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

    // To improve, make this 
    private void showWeaponsReticle(){
         RaycastHit hit;  // Store raycast hit information

        // If helicopter forward intersects environment, use ray cast
        if (Physics.Raycast(heli.transform.position, heli.transform.forward, out hit, 100f, collisionLayer))
        {
            rayCastReticle(hit);
        }
        else
        {
            // Otherwise, just project it in front of the helicopter
            projectReticle();

        }
    }

    private void rayCastReticle(RaycastHit hit){

        Vector3 crossHairsPosition = Vector3.Lerp(heli.transform.position, hit.point, .5f);

        crossHairs.transform.position = crossHairsPosition;

        // Billboard Baggins
        // Align the crosshair to camera, fix rotation offset
        Quaternion surfaceRotation = Quaternion.LookRotation(hit.point-mainCamera.transform.position);
        surfaceRotation = surfaceRotation * Quaternion.Euler(-90f, 0f, 0f); // Rotate by 90 degrees on X axis
        crossHairs.transform.rotation = surfaceRotation;
    }

    private void projectReticle(){

        crossHairs.transform.position = heli.transform.position + heli.transform.forward * 20f;

        // Billboard Baggins
        // Align the crosshair to camera, fix rotation offset
        Quaternion crosshairsRotation =  Quaternion.LookRotation(crossHairs.transform.position-mainCamera.transform.position);
        crosshairsRotation= crosshairsRotation * Quaternion.Euler(-90f, 0f, 0f); // Rotate by 90 degrees on X axis
        crossHairs.transform.rotation = crosshairsRotation;
    }
}
