using UnityEngine;
using System.Collections;

public class ClickToMoveTarget : MonoBehaviour
{
    Vector3 newPosition;

    [SerializeField] Camera cam;
	void Start () {
        newPosition = transform.position;
	}
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                newPosition = new Vector3(hit.point.x,10f,hit.point.z);
                transform.position = newPosition;
            }
        }
    }
}
