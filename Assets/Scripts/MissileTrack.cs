using System;
using UnityEngine;


public class MissileTrack : MonoBehaviour
{
    [Header("REFERENCES")]
    private Rigidbody _rb;
    [SerializeField] private GameObject _explosionPrefab;

    private GameObject _target;

    [Header("MOVEMENT")]
    [SerializeField] private float _speed = 15;
    [SerializeField] private float _rotateSpeed = 95;

    [Header("PREDICTION")]
    [SerializeField] private float _maxDistancePredict = 100;
    [SerializeField] private float _minDistancePredict = 5;
    [SerializeField] private float _maxTimePrediction = 5;
    [SerializeField] private float trackingRange = 100f;  // Optional: Range to consider targets within a certain distance
    private Vector3 _standardPrediction, _deviatedPrediction;
    

    [Header("DEVIATION")]
    [SerializeField] private float _deviationAmount = 50;
    [SerializeField] private float _deviationSpeed = 2;

    [Header("Lifetime")]
    [SerializeField] private float _lifetime = 5f;

    private float _randomSeed; // Random seed for each missile
    private float timer; // A timer counting down from lifetime to 0

    private float targetDistance = 110000f;

    private float launchDistance;
    private Boolean launchDistanceSet = false;

    private void Start()
    {
        // Assign a unique random seed to each missile at the start
        _randomSeed = UnityEngine.Random.Range(0f, 10000f); // Randomize the seed
        timer = _lifetime;
    }

    private void FixedUpdate()
    {
        _rb=GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.forward * _speed;

        _target = findClosestTarget();

        var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));
        //var leadTimePercentage = 1f;

        PredictMovement(leadTimePercentage);

        AddDeviation();

        RotateRocket();

        // Decrease the timer by the time passed since the last frame
        timer -= Time.deltaTime;

        // If the timer runs out, destroy the missile
        if (timer <= 0f)
        {
            DestroyMissile();
        }
    }


    private GameObject findClosestTarget()
    {
        // Find all Target GameObjects in the scene (you can use a tag or find all objects)
        GameObject[] targets = GameObject.FindGameObjectsWithTag("target");

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        

        foreach (GameObject t in targets)
        {
            targetDistance = Vector3.Distance(transform.position, t.transform.position);

            // Only consider targets within a certain range (optional)
            if (targetDistance < trackingRange && targetDistance < closestDistance)
            {
                closestDistance = targetDistance;
                closestTarget = t;
            }
        }

        if (!launchDistanceSet){
            launchDistance = targetDistance;
            launchDistanceSet = true;
        }

        return closestTarget;
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

        Rigidbody target_RB = _target.GetComponent<Rigidbody>();

        _standardPrediction = target_RB.position + target_RB.linearVelocity * predictionTime;
    }

    private void AddDeviation()
    {
        
        var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), Mathf.Cos(Time.time * _deviationSpeed), Mathf.Cos(Time.time * _deviationSpeed));

        float noise = Mathf.PerlinNoise(_randomSeed + Time.time * _deviationSpeed, 0f);

        // Scale noise so it gets weaker as it approaches target
        float distanceScale = Mathf.Lerp(0,1f,targetDistance/launchDistance);

        var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount  * distanceScale * noise;

        _deviatedPrediction = _standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = _deviatedPrediction - transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        //if (collision.transform.TryGetComponent<IExplode>(out var ex)) ex.Explode();

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _standardPrediction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
    }

    // Method to handle the destruction of the missile
    private void DestroyMissile()
    {
        Destroy(gameObject);  // Destroy the missile game object
    }
}
