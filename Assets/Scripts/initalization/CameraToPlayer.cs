using Unity.Cinemachine;
using UnityEngine;

// Script just sets the cinemachine camera target to the player's helicopter upon start, so I don't have to set it every time.
public class CameraToPlayer : MonoBehaviour
{
    GameObject playerObj;

    void Start()
    {
        // Find the Game Object with the player tag, get it's only child, the helicopter object
        playerObj = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        // Set the tracking target of the cinemachine target to that helicopter object
        GetComponent<CinemachineCamera>().Target.TrackingTarget = playerObj.transform;
    }

}
