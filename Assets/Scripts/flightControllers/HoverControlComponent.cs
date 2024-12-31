using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class HoverControlComponent 
{
    public FlightController FlightController { get; protected set; }
    public GameObject Helicopter { get { return FlightController.gameObject; } }
    public GameObject Target { get; private set; }

    private float _targetAltitude;
    public float TargetAltitude
    {
        get { return _targetAltitude; }
        set
        {
            _targetAltitude = value;
            altitudePID.Target = value;
        }
    }

    public Vector3 TargetHeading;


    protected PID pitchPID;
    protected PID rollPID;
    protected PID yawPID;
    protected PID altitudePID;

    protected PID linearXPID;
    protected PID linearZPID;

    private float _targetForwardVelocity;
    public float TargetForwardVelocity
    {
        get { return _targetForwardVelocity; }
        set
        {
            linearZPID.Target = value;
            _targetForwardVelocity = value;
        }
    }
    private float _targetLateralVelocity;
    public float TargetLateralVelocity
    {
        get { return _targetLateralVelocity; }
        set
        {
            linearXPID.Target = value;
            _targetLateralVelocity = value;
        }
    }

    public HoverControlComponent(FlightController flightController)
    {
        FlightController = flightController;
        Target = new GameObject();

        pitchPID = new PID();
        pitchPID.Kp = 0.85f;
        pitchPID.Kd = 1f;
        pitchPID.Ki = 0f;

        rollPID = new PID();
        rollPID.Kp = 0.85f;
        rollPID.Kd = 1f;
        rollPID.Ki = 0f;

        yawPID = new PID();
        yawPID.Kp = 0.05f;
        yawPID.Kd = 0.2f;
        yawPID.Ki = 0f;
        yawPID.IntegralLimit = 1f;

        altitudePID = new PID();
        altitudePID.Kp = 1f;
        altitudePID.Kd = 200f;
        altitudePID.Ki = 0.1f;
        altitudePID.IntegralLimit = 5f;

        linearXPID = new PID();
        linearXPID.Kp = 1f;
        linearXPID.Kd = 2f;
        linearXPID.Ki = 0.5f;
        linearXPID.IntegralLimit = 10;

        linearZPID = new PID();
        linearZPID.Kp = 1f;
        linearZPID.Kd = 2f;
        linearZPID.Ki = 0.5f;
        linearZPID.IntegralLimit = 10;
    }

    public void Update()
    {
        Vector3 attitudeError = GetAttitudeError();
        float pitchControl = pitchPID.Update(Time.fixedDeltaTime, attitudeError.x);
        pitchControl = Mathf.Clamp(pitchControl, -1, 1);

        float rollControl = rollPID.Update(Time.fixedDeltaTime, attitudeError.y);
        rollControl = Mathf.Clamp(rollControl, -1, 1);

        float yawControl = yawPID.Update(Time.fixedDeltaTime, attitudeError.z);
        yawControl = Mathf.Clamp(yawControl, -1, 1);

        float altitudeControl = altitudePID.Update(Time.fixedDeltaTime, Helicopter.transform.position.y);
        altitudeControl = Mathf.Clamp(altitudeControl, -1, 1);

        //Debug.Log($"Pitch:{pitchControl:0.00} | Roll:{rollControl:0.00} | Yaw:{yawControl:0.00} | Alt:{altitudeControl:0.00}");

        FlightController.ApplyPitch(pitchControl);
        FlightController.ApplyRoll(rollControl);
        FlightController.ApplyYaw(yawControl);
        FlightController.ApplyLift(altitudeControl);

    }

    private Vector3 GetAttitudeError()
    {
        Vector3 velocityError = GetVelocityError();

        Quaternion targetAttitude = Quaternion.FromToRotation(Helicopter.transform.up, Vector3.up) * Helicopter.transform.rotation;
        //targetAttitude *= Quaternion.Euler(-TargetPitch, 0, -TargetRoll);
        targetAttitude *= Quaternion.Euler(velocityError.x, 0, velocityError.z);
        targetAttitude = targetAttitude.normalized;

        Target.transform.position = Helicopter.transform.position;
        Target.transform.rotation = targetAttitude;

        float pitchError = Vector3.SignedAngle(Helicopter.transform.forward, Target.transform.forward, Helicopter.transform.right);
        float rollError = Vector3.SignedAngle(Helicopter.transform.right, Target.transform.right, Helicopter.transform.forward);


        Quaternion targetOrientation = Quaternion.FromToRotation(Helicopter.transform.forward, TargetHeading) * targetAttitude;
        targetOrientation = targetOrientation.normalized;

        float yawSign = -Mathf.Sign(Vector3.SignedAngle(Target.transform.forward, TargetHeading, Vector3.up));
        float yawError = Quaternion.Angle(targetAttitude, targetOrientation);
        yawError *= yawSign;

        //Debug.Log($"Pitch:{pitchError:0.00} | Roll:{rollError:0.00} | Yaw:{yawError:0.00}");
        DebugDrawTransform();

        return new Vector3(pitchError, rollError, yawError);
    }

    private Vector3 GetVelocityError()
    {
        Rigidbody helicopterBody = Helicopter.GetComponent<Rigidbody>();
        Vector3 planarVelocityLocal = Helicopter.transform.InverseTransformDirection(helicopterBody.linearVelocity);
        Debug.Log($"X:{planarVelocityLocal.x:0.00} | Z:{planarVelocityLocal.z:0.00}");

        float pitchAngle = linearXPID.Update(Time.fixedDeltaTime, planarVelocityLocal.x);
        float rollAngle = linearZPID.Update(Time.fixedDeltaTime, -planarVelocityLocal.z);

        Vector3 targetAngles = new Vector3(-rollAngle, 0, -pitchAngle);
        //Debug.Log($"Pitch:{pitchAngle:0.00} | Roll:{rollAngle:0.00}");

        return targetAngles;
    }

    private void DebugDrawTransform()
    {
        float magnitude = 10;
        Transform transform = Target.transform;
        Vector3 pos = transform.position;

        Debug.DrawLine(pos, pos + transform.forward * magnitude, Color.blue);
        Debug.DrawLine(pos, pos + transform.up * magnitude, Color.green);
        Debug.DrawLine(pos, pos + transform.right * magnitude, Color.red);
    }
}
