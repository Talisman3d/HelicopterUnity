using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Mathematics;

// Holds all possible weapon systems and behaviors
// Probably needs a refactor so that each weapon launcher has a behavior for its weapon
public class WeaponController : MonoBehaviour
{
    [Header("Set Prefabs")]

    
    
    
    [SerializeField] GameObject quad; 

    public GameObject target; // target for missile to track

    [Header("Bullet Settings")]
    [SerializeField] GameObject bullet; // Eventually I'd like this to be modulars
    [SerializeField] GameObject bulletCasing; 
    [SerializeField] float bulletVelocity = 10.0f;
    [SerializeField] float bulletCooldown = 0.2f;
    [SerializeField] float bulletRandom = 200f;
    [SerializeField] float bulletRecoil = 100f;
    [SerializeField] float torqueMax =20f;
    [SerializeField] float casingForce = 20f;
    AudioSource gunFiringSound;
    float timeSinceBullet = 0.0f;

    [Header("Rocket Settings")]
    [SerializeField] GameObject rocket; // Eventually I'd like this to be modulars
    [SerializeField] float rocketVelocity = 10.0f;
    [SerializeField] float rocketCooldown = 5f;
    [SerializeField] float rocketRandom = 200f;
    [SerializeField] float rocketRecoil = 200f;
    AudioSource rocketLaunchSound;
    float timeSinceRocket = 5f;

    [Header("Missile Settings")]
    [SerializeField] GameObject missile; // Eventually I'd like this to be modulars
    [SerializeField] float missileLaunchVelocity = 300f;
    [SerializeField] float missileTiming = 0.5f;
    [SerializeField] float missileCooldown = 2f;
    float timeSinceMissile= 5f;

    [Header("Harpoon Settings")]
    [SerializeField] GameObject grapplingHook;
    [SerializeField] float harpoonLaunchVelocity = 10f;


    // Parent References
    GameObject controller;
    GameObject helicopter;
    Rigidbody parentRB; // Helicopter parent's Rigid Body (for recoil)

    // Other script references
    MissileTrack missileTrackScript; // MoveToTargetScript
    FindClosestTarget findClosestTarget;

    void Start()
    {        
        // References to weapon launcher objects, WIP
        GameObject gau = this.transform.Find("GauCannon").gameObject; // Eventually I want this to be modular
        GameObject rkt = this.transform.Find("rocketLauncher").gameObject; // Eventually I want this to be modular
        
        // References to audiosources per weapon object
        rocketLaunchSound=rkt.GetComponent<AudioSource>(); 
        gunFiringSound=gau.GetComponent<AudioSource>();

        // Set parent references
        helicopter = transform.parent.gameObject;
        controller = helicopter.transform.parent.gameObject;
        parentRB =helicopter.GetComponent<Rigidbody>(); // Set Helicopter Rigid Body (for recoil)
        findClosestTarget = helicopter.GetComponent<FindClosestTarget>();
    }

    private void FixedUpdate() {
        // Handle cooldowns
        timeSinceRocket += Time.fixedDeltaTime; // Update rocket cooldown
        timeSinceBullet+=Time.fixedDeltaTime; // Update time between bullet fires again
        timeSinceMissile+=Time.fixedDeltaTime; // Update time between bullet fires again

        // Scan for closest target
        target=findClosestTarget.closestTarget;
    }

    public void fireGun(){
            if (timeSinceBullet>bulletCooldown){

                // Spawn and shoot bullet
                GameObject bulletInstance = Instantiate(bullet, transform.position,transform.rotation);
                Vector3 bulletForce = new Vector3 ( UnityEngine.Random.Range(-bulletRandom, bulletRandom), UnityEngine.Random.Range(-bulletRandom, bulletRandom) ,bulletVelocity);
                bulletInstance.GetComponent<Rigidbody>().AddRelativeForce(bulletForce);
                parentRB.AddRelativeForce(-bulletForce*bulletRecoil);
                timeSinceBullet=0f;

                // Spawn and drop bullet casing
                GameObject bulletCasingInstance = Instantiate(bulletCasing, transform.position, transform.rotation);
                //bulletCasingInstance.GetComponent<Rigidbody>().linearVelocity=helicopter.GetComponent<Rigidbody>().linearVelocity;
                
                Vector3 randomTorque = new Vector3(
                    UnityEngine.Random.Range(-torqueMax, torqueMax),
                    UnityEngine.Random.Range(-torqueMax, torqueMax),
                    UnityEngine.Random.Range(-torqueMax, torqueMax)
                );
                
                bulletCasingInstance.GetComponent<Rigidbody>().AddRelativeTorque(randomTorque);
                Vector3 bulletCasingForce = new Vector3 ( UnityEngine.Random.Range(0, casingForce), UnityEngine.Random.Range(-casingForce, casingForce) ,-10);
                bulletCasingInstance.GetComponent<Rigidbody>().AddRelativeForce(bulletCasingForce);
                Destroy(bulletCasingInstance, 2f);
            }
         
    }

    public void startGunSound(){
        // So I can access elewhere
        if (!gunFiringSound.isPlaying){
            gunFiringSound.Play();
            gunFiringSound.loop = true;   
        }
 
    }
    public void stopGunSound(){
        gunFiringSound.Stop();
    }
    

    public void fireRockets(){
        GameObject rocketInstance = Instantiate(rocket, transform.position, transform.rotation);
        Vector3 rocketVector = new Vector3 ( UnityEngine.Random.Range(-rocketRandom, rocketRandom), UnityEngine.Random.Range(-rocketRandom, rocketRandom) ,rocketVelocity);
        rocketInstance.GetComponent<Rigidbody>().AddRelativeForce(rocketVector);
        parentRB.AddRelativeForce(-rocketVector*rocketRecoil);
        
        rocketLaunchSound.Play();
        timeSinceRocket+=Time.fixedDeltaTime;
    }

    public IEnumerator rocketVolley()
    {
        // This is how you accomplish delays between for loop iterations
        // I wanted a behavior where you hit a button once and multiple projectiles launch, each slightly delayed
        if (timeSinceRocket > rocketCooldown){
            for (int i=0;i<4;i++){
            
                fireRockets();
                yield return new WaitForSeconds(0.1f);
            
            }   
            timeSinceRocket = 0;
        }
    }

    public IEnumerator missileVolley()
    {
        // This is how you accomplish delays between for loop iterations
        // I wanted a behavior where you hit a button once and multiple projectiles launch, each slightly delayed
        if (timeSinceMissile > missileCooldown){
            //for (int i=0;i<3;i++){
                GameObject missileGameObject= fireMissile();
                yield return new WaitForSeconds(missileTiming);
                if (missileGameObject){
                    missileTrack(missileGameObject);
                }
                
            //}   
            timeSinceMissile = 0;
        }
    }

    private GameObject fireMissile(){
        // Initial launch of missile, fire downwards
        GameObject missileInstance = Instantiate(missile, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
        Vector3 missileVector = new Vector3 (0,-missileLaunchVelocity ,0);
        missileInstance.GetComponent<Rigidbody>().AddRelativeForce(missileVector);
        return missileInstance;
    }

    private void missileTrack(GameObject missileInstance){
        missileTrackScript = missileInstance.GetComponent<MissileTrack>(); // Grab the missile track script
        missileTrackScript.initialization(target);  // set missiles target object
        rocketLaunchSound.Play(); //whooosh
        Vector3 missileVector = new Vector3 (0,missileLaunchVelocity ,0); // Add force down to cancel out upwards launch
        missileInstance.GetComponent<Rigidbody>().AddRelativeForce(missileVector);
        missileTrackScript.enabled = true; // Start tracking target
        missileInstance.GetComponent<TrailRenderer>().enabled=true; // Add a nice tail to the missile
    }

    public void launchQuad(){
        GameObject quadInstance = Instantiate(quad, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
    }

    public void fireGrapplingHook(){
        GameObject grapplingInstance = Instantiate(grapplingHook, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
        grapplingInstance.GetComponent<GrapplingHook>().initialization(helicopter, controller.tag);
        Vector3 grapplingVector = new Vector3 (0,0,harpoonLaunchVelocity);
        grapplingInstance.GetComponent<Rigidbody>().AddRelativeForce(grapplingVector);
    }
    

}
