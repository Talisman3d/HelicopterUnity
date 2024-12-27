using UnityEngine;

// Applying Wind to all helicopter objects in level
public class WindGusts : MonoBehaviour
{
    [SerializeField] float windMax = 2000f;
    GameObject[] rotorCraft;
    Vector3 windVec;

    [SerializeField] float slope=-10;
    void Start()
    {
        rotorCraft = GameObject.FindGameObjectsWithTag("Player");
    }
    void FixedUpdate()
    {
        windVec=calcWind();
        applyWind(windVec);
    }

    Vector3 calcWind(){
        float windVal = RandomFromDistribution.RandomRangeLinear(0, windMax, slope); // I found this library on asset store
        windVec = Vector3.left * windVal;
        return windVec;
    }

    private void applyWind(Vector3 windVec){
        for(int i=0;i<rotorCraft.Length;i++){
            rotorCraft[i].GetComponent<Rigidbody>().AddRelativeForce(windVec * Time.fixedDeltaTime);
        }
    }


}
