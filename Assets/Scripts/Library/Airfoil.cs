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
    public float AreaScale { get; protected set; } = 1;

    public float AirVelocity { get; protected set; }
    public float AngleOfAttack { get; protected set; }
    public Rigidbody Rigidbody { get; protected set; }

    // Use this for initialization
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        AirVelocity = CalculateAirVelocity();
        AngleOfAttack = CalculateAngleOfAttack();
        ApplyForces();
    }

    protected virtual float CalculateAirVelocity()
    {
        Vector3 velocityLocalized = transform.InverseTransformDirection(Rigidbody.linearVelocity);
        Vector3 forwardAxisLocalized = ForwardAxis.normalized;
        float airVelocity = Vector3.Dot(velocityLocalized, forwardAxisLocalized);
        return airVelocity;
    }

    protected virtual float CalculateAngleOfAttack()
    {
        Vector3 velocityLocalized = transform.InverseTransformDirection(Rigidbody.linearVelocity);
        Vector3 forwardAxisLocalized = ForwardAxis.normalized;

        Vector3 normalAxis = Vector3.Cross(ForwardAxis, UpAxis);
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(velocityLocalized, normalAxis);
        float angleOfAttack = Vector3.SignedAngle(projectedVelocity, forwardAxisLocalized, normalAxis);
        return angleOfAttack;
    }

    protected virtual Vector3 CalculateLiftForce()
    {
        float liftCoefficient = LiftProfile.Evaluate(AngleOfAttack);
        float liftForce = CalculateAerodynamicForce(liftCoefficient, AirVelocity, AreaScale); 
        Vector3 liftVector = UpAxis * liftForce;
        return liftVector;
    }

    protected virtual Vector3 CalculateDragForce()
    {
        float dragCoefficient = DragProfile.Evaluate(AngleOfAttack);
        float dragForce = CalculateAerodynamicForce(dragCoefficient, AirVelocity, AreaScale);
        Vector3 dragVector = ForwardAxis * dragForce * -Mathf.Sign(AirVelocity);
        return dragVector;
    }

    protected virtual void ApplyForces()
    {
        if (AirVelocity == 0) return;
        Vector3 liftVector = CalculateLiftForce();
        Vector3 dragVector = CalculateDragForce();

        Rigidbody.AddRelativeForce(liftVector * Time.fixedDeltaTime);
        Rigidbody.AddRelativeForce(dragVector * Time.fixedDeltaTime);

        /*
        Vector3 pos = transform.position;
        float scale = 2f;
        Debug.DrawLine(pos, transform.TransformPoint(liftVector * scale), Color.magenta);
        Debug.DrawLine(pos, transform.TransformPoint(dragVector * scale), Color.yellow);
        */
    }

    public static float CalculateAerodynamicForce(float coefficient, float velocity, float referenceArea)
    {
        return (coefficient * AirDensity * Mathf.Pow(velocity, 2) * referenceArea) * 0.5f;
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
