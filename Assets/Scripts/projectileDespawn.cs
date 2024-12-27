using UnityEngine;

// Handles projectile life and impact
public class projectileDespawn : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision other) {
        if (!audioSource.isPlaying){
            audioSource.Play();
        }
        
    }

}
