using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class Airfoil : MonoBehaviour
{
    public static readonly float AirDensity = 1.293f; // kg-m3

    [field: SerializeField]
    public Vector3 ForwardAxis { get; protected set; } = Vector3.forward;
    [field: SerializeField]
    public Vector3 UpAxis { get; protected set; } = Vector3.up;

    [field: SerializeField]
    public AnimationCurve LiftProfile { get; protected set; } = CreateBasicLiftProfile();
    [field: SerializeField]
    public AnimationCurve DragProfile { get; protected set; } = CreateBasicDragProfile();
    [field: SerializeField]
    public float AreaScale { get; protected set; } = 10;

    public float AirVelocity { get; private set; }
    public float AngleOfAttack { get; private set; }
    public Rigidbody Rigidbody { get; protected set; }

    // Use this for initialization
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirfoilState();
        ApplyForces();
    }

    private void UpdateAirfoilState()
    {
        Vector3 velocityLocalized = transform.InverseTransformDirection(Rigidbody.linearVelocity);
        Vector3 forwardAxisLocalized = ForwardAxis.normalized;
        AirVelocity = Vector3.Dot(velocityLocalized, forwardAxisLocalized);

        Vector3 normalAxis = Vector3.Cross(ForwardAxis, UpAxis);
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocityLocalized, normalAxis);
        AngleOfAttack = Vector3.SignedAngle(projectedVelocity, forwardAxisLocalized, normalAxis);

        Debug.Log($"{velocityLocalized.ToString():0.00}|{AirVelocity:0.00}|{AngleOfAttack:0.00}");
        
        Debug.DrawLine(transform.position, transform.TransformPoint(forwardAxisLocalized * AirVelocity));
        Debug.DrawLine(transform.position, transform.TransformPoint(projectedVelocity) * 1, Color.black);
        Debug.DrawLine(transform.position, transform.TransformPoint(normalAxis) * 1, Color.gray);
        
    }


    private void ApplyForces()
    {
        if (AirVelocity == 0) return;
        float liftCoefficient = LiftProfile.Evaluate(AngleOfAttack);
        float liftForce = (liftCoefficient * AirDensity * Mathf.Pow(AirVelocity, 2) * AreaScale) * 0.5f;
        Vector3 liftVector = UpAxis * liftForce;

        float dragCoefficient = DragProfile.Evaluate(AngleOfAttack);
        float dragForce = (dragCoefficient * AirDensity * Mathf.Pow(AirVelocity, 2) * AreaScale) * 0.5f;
        Vector3 dragVector = ForwardAxis * dragForce * -Mathf.Sign(AirVelocity);

        Rigidbody.AddRelativeForce(liftVector * Time.fixedDeltaTime);
        Rigidbody.AddRelativeForce(dragVector * Time.fixedDeltaTime);

        Vector3 pos = transform.position;
        float scale = 2f;
        Debug.DrawLine(pos, transform.TransformPoint(liftVector * scale), Color.magenta);
        Debug.DrawLine(pos, transform.TransformPoint(dragVector * scale), Color.yellow);
    }

    public static AnimationCurve CreateBasicLiftProfile()
    {
        //ref NACA 0018
        AnimationCurve profile = new AnimationCurve();
        profile.AddKey(-25f, -0.2f);
        profile.AddKey(-20f, -1.1f);
        profile.AddKey(-15f, -1.1f);
        profile.AddKey(0f, 0f);
        profile.AddKey(15f, 1.1f);
        profile.AddKey(20f, 1.1f);
        profile.AddKey(25f, 0.2f);
        return profile;
    }

    public static AnimationCurve CreateBasicDragProfile()
    {
        //ref NACA 0018
        AnimationCurve profile = new AnimationCurve();
        profile.AddKey(-20f, 0.1f);
        profile.AddKey(-16f, 0.06f);
        profile.AddKey(0f, 0.01f);
        profile.AddKey(16f, 0.06f);
        profile.AddKey(20f, 0.1f);
        return profile;
    }
}
