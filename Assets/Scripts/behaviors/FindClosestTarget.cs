using UnityEngine;

public class FindClosestTarget : MonoBehaviour
{
    public GameObject closestTarget;
    public string targetTag = "Target";

    private void Update() {
         closestTarget = scanForClosestTarget();
    }

    private GameObject scanForClosestTarget(){
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        if (targets.Length>0){
            foreach (GameObject t in targets)
            {
                float targetDistance = Vector3.Distance(transform.position, t.transform.position);

                // Only consider targets within a certain range (optional)
                if (targetDistance < closestDistance)
                {
                    closestDistance = targetDistance;
                    closestTarget = t;
                }
            }            
        }

        return closestTarget;
    }

    
}
