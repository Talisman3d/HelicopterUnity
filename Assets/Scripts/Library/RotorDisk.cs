using System;
using Unity;
using Unity.Cinemachine;
using UnityEngine;

public class RotorDisk : Airfoil
{
    protected override float CalculateAirVelocity()
    {
        return ProjectedVelocityOnDisk().magnitude;
    }

    protected override float CalculateAngleOfAttack()
    {
        Vector3 velocityLocalized = transform.InverseTransformDirection(Rigidbody.linearVelocity);
        Vector3 velocityProjected = velocityLocalized.ProjectOntoPlane(UpAxis);

        Vector3 referenceAxis = Vector3.Cross(velocityProjected, UpAxis);
        Vector3 referenceVelocity = velocityLocalized.ProjectOntoPlane(referenceAxis);

        float angleOfAttack = Vector3.SignedAngle(referenceVelocity, velocityProjected, referenceAxis);

        return angleOfAttack;
    }

    protected override Vector3 CalculateDragForce()
    {
        float dragCoefficient = DragProfile.Evaluate(AngleOfAttack);
        float dragForce = CalculateAerodynamicForce(dragCoefficient, AirVelocity, AreaScale);

        Vector3 dragDirection = ProjectedVelocityOnDisk().normalized;

        Vector3 dragVector = dragDirection * dragForce * -Mathf.Sign(AirVelocity);
        return dragVector;
    }

    private Vector3 ProjectedVelocityOnDisk()
    {
        Vector3 velocityLocalized = transform.InverseTransformDirection(Rigidbody.linearVelocity);
        Vector3 velocityProjected = velocityLocalized.ProjectOntoPlane(UpAxis);
        return velocityProjected;
    }
}

