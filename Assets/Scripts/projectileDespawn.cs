using UnityEngine;

// Handles projectile life and impact

public class projectileDespawn : MonoBehaviour
{
    [SerializeField] float lifeTime = 5f;
    AudioSource audioSource;
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision other) {
        if (gameObject){
            if (!audioSource.isPlaying){
                audioSource.Play();
            }
        }

    }

}
