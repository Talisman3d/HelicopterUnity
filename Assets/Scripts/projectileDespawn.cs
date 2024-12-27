using UnityEngine;

public class projectileDespawn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    AudioSource audioSource;
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other) {
        audioSource.Play();
        if (!audioSource.isPlaying){
        }
        
    }

}
