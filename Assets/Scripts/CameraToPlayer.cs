using Unity.Cinemachine;
using UnityEngine;

public class CameraToPlayer : MonoBehaviour
{
    GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
        GetComponent<CinemachineCamera>().Target.TrackingTarget = playerObj.transform;
    }

}
