using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Interprets input to fire weapons
// WIP, needs to be abstracted
public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject bullet; // Eventually I'd like this to be modulars
    [SerializeField] GameObject rocket; // Eventually I'd like this to be modulars
    [SerializeField] GameObject missile; // Eventually I'd like this to be modulars

    [SerializeField] GameObject grapplingHook;

    [SerializeField] GameObject quad; 

    public GameObject target; // target for missile to track

    // Primary settings
    [SerializeField] float bulletVelocity = 10.0f;
    [SerializeField] float bulletCooldown = 0.2f;
    [SerializeField] float bulletRandom = 200f;
    [SerializeField] float bulletRecoil = 100f;
    AudioSource gunFiringSound;
    float timeSinceBullet = 0.0f;

    // Secondary settings
    [SerializeField] float rocketVelocity = 10.0f;
    [SerializeField] float rocketCooldown = 5f;
    [SerializeField] float rocketRandom = 200f;
    [SerializeField] float rocketRecoil = 200f;
    AudioSource rocketLaunchSound;
    float timeSinceRocket = 5f;

    [SerializeField] float missileLaunchVelocity = 300f;
    [SerializeField] float missileTiming = 0.5f;
    [SerializeField] float missileCooldown = 2f;
    float timeSinceMissile= 5f;

    [SerializeField] float harpoonLaunchVelocity = 10f;

    GameObject helicopter;
    Rigidbody parentRB; // Helicopter parent's Rigid Body (for recoil)

    MissileTrack missileTrackScript; // MoveToTargetScript

    void Start()
    {        
        GameObject gau = this.transform.Find("GauCannon").gameObject; // Eventually I want this to be modular
        GameObject rkt = this.transform.Find("rocketLauncher").gameObject; // Eventually I want this to be modular
        
        rocketLaunchSound=rkt.GetComponent<AudioSource>(); 
        gunFiringSound=gau.GetComponent<AudioSource>();

        //StartCoroutine(rocketVolley()); // Needed to do delay for loop
        //StartCoroutine(missileVolley()); // Needed to do delay for loop

        helicopter = transform.parent.gameObject;
        parentRB =helicopter.GetComponent<Rigidbody>(); // Set Helicopter Rigid Body (for recoil)
    }

    private void FixedUpdate() {
        timeSinceRocket += Time.fixedDeltaTime; // Update rocket cooldown
        timeSinceBullet+=Time.fixedDeltaTime; // Update time between bullet fires again
        timeSinceMissile+=Time.fixedDeltaTime; // Update time between bullet fires again
    }

    public void fireGun(){
            if (timeSinceBullet>bulletCooldown){
                GameObject bulletInstance = Instantiate(bullet, transform.position,  
                                                     transform.rotation);
                Vector3 bulletForce = new Vector3 
                                                (Random.Range(-bulletRandom, bulletRandom),Random.Range(-bulletRandom, bulletRandom) ,bulletVelocity);
                bulletInstance.GetComponent<Rigidbody>().AddRelativeForce(bulletForce);
                parentRB.AddRelativeForce(-bulletForce*bulletRecoil);
                timeSinceBullet=0f;
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
        Vector3 rocketVector = new Vector3 (Random.Range(-rocketRandom, rocketRandom),Random.Range(-rocketRandom, rocketRandom) ,rocketVelocity);
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
        // Initial launch of missile, fire upwards
        GameObject missileInstance = Instantiate(missile, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
        Vector3 missileVector = new Vector3 (0,-missileLaunchVelocity ,0);
        missileInstance.GetComponent<Rigidbody>().AddRelativeForce(missileVector);
        return missileInstance;
    }

    private void missileTrack(GameObject missileInstance){
        missileTrackScript = missileInstance.GetComponent<MissileTrack>();
        
        rocketLaunchSound.Play();
        // Add force down to cancel out upwards launch
        Vector3 missileVector = new Vector3 (0,missileLaunchVelocity ,0);
        missileInstance.GetComponent<Rigidbody>().AddRelativeForce(missileVector);
        missileTrackScript.enabled = true;
        missileInstance.GetComponent<TrailRenderer>().enabled=true;
    }

    public void launchQuad(){
        GameObject quadInstance = Instantiate(quad, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
    }

    public void fireGrapplingHook(){
        
        GameObject grapplingInstance = Instantiate(grapplingHook, new Vector3(transform.position.x,transform.position.y-0.5f,transform.position.z), transform.rotation);
        grapplingHook.GetComponent<GrapplingHook>().helicopter = helicopter;
        Vector3 grapplingVector = new Vector3 (0,0,harpoonLaunchVelocity);
        grapplingInstance.GetComponent<Rigidbody>().AddRelativeForce(grapplingVector);
    }
    

}
