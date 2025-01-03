using UnityEngine;

public class RenderReticle : MonoBehaviour
{
    public LayerMask collisionLayer;
    public GameObject crossHairsPrefab;   // Reference to the dot prefab
    private GameObject crossHairs;        // Instance of the dot
    public Camera mainCamera;         // Reference to the main camera

    // Crosshairs material shtuff
    public Color emissionColor = Color.red;  
    public float emissionIntensity = 1.0f;    
    public Color baseColor = Color.white;  

    void Start()
    {
        // Instantiate Crosshairs in front of helicopter
        crossHairs = Instantiate(crossHairsPrefab, Vector3.zero, Quaternion.identity);

        Renderer chRenderer = crossHairs.GetComponent<Renderer>();

        // If a renderer is found and the material supports emission, update the emission color
        if (chRenderer != null)
        {
            Material chMat = chRenderer.material;

            // Enable emission if it's not already enabled
            chMat.EnableKeyword("_EMISSION");

            // Set the emission color and intensity
            chMat.SetColor("_EmissionColor", emissionColor * emissionIntensity);

                // Set the base color (albedo or diffuse color)
            chMat.SetColor("_Color", baseColor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        showWeaponsReticle();
    }

    
    private void showWeaponsReticle(){
         RaycastHit hit;  // Store raycast hit information

        // If helicopter forward intersects environment, use ray cast
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, collisionLayer))
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

        Vector3 crossHairsPosition = Vector3.Lerp(transform.position, hit.point, .5f);

        crossHairs.transform.position = crossHairsPosition;

        // Billboard Baggins
        // Align the crosshair to camera, fix rotation offset
        Quaternion surfaceRotation = Quaternion.LookRotation(hit.point-mainCamera.transform.position);
        surfaceRotation = surfaceRotation * Quaternion.Euler(-90f, 0f, 0f); // Rotate by 90 degrees on X axis
        crossHairs.transform.rotation = surfaceRotation;
    }

    private void projectReticle(){

        crossHairs.transform.position = transform.position + transform.forward * 20f;

        // Billboard Baggins
        // Align the crosshair to camera, fix rotation offset
        Quaternion crosshairsRotation =  Quaternion.LookRotation(crossHairs.transform.position-mainCamera.transform.position);
        crosshairsRotation= crosshairsRotation * Quaternion.Euler(-90f, 0f, 0f); // Rotate by 90 degrees on X axis
        crossHairs.transform.rotation = crosshairsRotation;
    }
}
