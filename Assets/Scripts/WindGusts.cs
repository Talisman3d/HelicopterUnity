using UnityEngine;

public class WindGusts : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float windMax = 2000f;
    GameObject[] rotorCraft;
    Vector3 windVec;

    [SerializeField] float slope=-10;
    void Start()
    {
        rotorCraft = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        windVec=calcWind();
        applyWind(windVec);
    }

    Vector3 calcWind(){
        float windVal = RandomFromDistribution.RandomRangeLinear(0, windMax, slope);
        windVec = Vector3.left * windVal;
        return windVec;
    }

    private void applyWind(Vector3 windVec){
        for(int i=0;i<rotorCraft.Length;i++){
            rotorCraft[i].GetComponent<Rigidbody>().AddRelativeForce(windVec * Time.fixedDeltaTime);
        }
    }


}
