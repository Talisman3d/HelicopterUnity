using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Weapons : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject rocket;

    [SerializeField] float bulletVelocity = 10.0f;
    [SerializeField] float bulletCooldown = 0.2f;
    [SerializeField] float bulletRandom = 200f;
    [SerializeField] float bulletRecoil = 100f;

    [SerializeField] float rocketVelocity = 10.0f;
    [SerializeField] float rocketCooldown = 5f;
    [SerializeField] float rocketRandom = 200f;
    [SerializeField] float rocketRecoil = 200f;


    AudioSource rocketLaunchSound;
    AudioSource gunFiringSound;

    float timeSinceBullet = 0.0f;
    float timeSinceRocket = 5f;

    Rigidbody parentRB;

    void Start()
    {        
        GameObject gau = this.transform.Find("GauCannon").gameObject;
        GameObject rkt = this.transform.Find("rocketLauncher").gameObject;
        
        rocketLaunchSound=rkt.GetComponent<AudioSource>();
        gunFiringSound=gau.GetComponent<AudioSource>();

        StartCoroutine(rocketVolley());

        parentRB = transform.parent.gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        timeSinceRocket += Time.fixedDeltaTime;
        timeSinceBullet+=Time.fixedDeltaTime;
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
        if (!gunFiringSound.isPlaying){
            gunFiringSound.Play();
            gunFiringSound.loop = true;   
        }
 
    }
    public void stopGunSound(){
        if (gunFiringSound.isPlaying){
            gunFiringSound.Stop();
        }

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
        if (timeSinceRocket > rocketCooldown){
            for (int i=0;i<4;i++){
            
                fireRockets();
                yield return new WaitForSeconds(0.05f);
            
            }   
            timeSinceRocket = 0;
        }
    }

}
