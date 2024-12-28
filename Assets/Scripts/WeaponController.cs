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

    Rigidbody parentRB; // Helicopter parent's Rigid Body (for recoil)

    void Start()
    {        
        GameObject gau = this.transform.Find("GauCannon").gameObject; // Eventually I want this to be modular
        GameObject rkt = this.transform.Find("rocketLauncher").gameObject; // Eventually I want this to be modular
        
        rocketLaunchSound=rkt.GetComponent<AudioSource>(); 
        gunFiringSound=gau.GetComponent<AudioSource>();

        StartCoroutine(rocketVolley()); // Needed to do delay for loop

        parentRB = transform.parent.gameObject.GetComponent<Rigidbody>(); // Set Helicopter Rigid Body (for recoil)
    }

    private void FixedUpdate() {
        timeSinceRocket += Time.fixedDeltaTime; // Update rocket cooldown
        timeSinceBullet+=Time.fixedDeltaTime; // Update time between bullet fires again
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

}
