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

    protected PID velocityXPID;
    protected PID velocityZPID;

    protected PID positionXPID;
    protected PID positionZPID;

    private float _targetForwardVelocity;
    public float TargetForwardVelocity
    {
        get { return _targetForwardVelocity; }
        set
        {
            velocityZPID.Target = value;
            _targetForwardVelocity = value;
        }
    }
    private float _targetLateralVelocity;
    public float TargetLateralVelocity
    {
        get { return _targetLateralVelocity; }
        set
        {
            velocityXPID.Target = value;
            _targetLateralVelocity = value;
        }
    }

    public Vector3? TargetPosition = null;

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

        velocityXPID = new PID();
        velocityXPID.Kp = 1f;
        velocityXPID.Kd = 2f;
        velocityXPID.Ki = 0.5f;
        velocityXPID.IntegralLimit = 10;

        velocityZPID = new PID();
        velocityZPID.Kp = 1f;
        velocityZPID.Kd = 2f;
        velocityZPID.Ki = 0.5f;
        velocityZPID.IntegralLimit = 10;

        positionXPID = new PID();
        positionXPID.Kp = 2f;
        positionXPID.Kd = 7500f;
        positionXPID.Ki = 0f;
        positionXPID.IntegralLimit = 10f;

        positionZPID = new PID();
        positionZPID.Kp = 1f;
        positionZPID.Kd = 5000f;
        positionZPID.Ki = 0f;
        positionZPID.IntegralLimit = 10f;
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
        Vector3 translationalError = GetTranslationalError();

        Quaternion targetAttitude = Quaternion.FromToRotation(Helicopter.transform.up, Vector3.up) * Helicopter.transform.rotation;
        //targetAttitude *= Quaternion.Euler(-TargetPitch, 0, -TargetRoll);
        targetAttitude *= Quaternion.Euler(translationalError.x, 0, translationalError.z);
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

        Debug.Log($"Pitch:{pitchError:0.00} | Roll:{rollError:0.00} | Yaw:{yawError:0.00}");
        DebugDrawTransform();

        return new Vector3(pitchError, rollError, yawError);
    }

    private Vector3 GetTranslationalError()
    {
        float velocityXControl = 0;
        float velocityZControl = 0;
        if (TargetPosition != null)
        {
            Vector3 positionError = CalculatePositionError(TargetPosition.Value);
            float positionXControl = positionXPID.Update(Time.fixedDeltaTime, positionError.x);
            float positionZControl = positionZPID.Update(Time.fixedDeltaTime, -positionError.z);

            velocityXControl = velocityXPID.Update(Time.fixedDeltaTime, positionXControl);
            velocityZControl = velocityZPID.Update(Time.fixedDeltaTime, positionZControl);
        }
        else
        {
            Vector3 velocityError = CalculateVelocityError();
            velocityXControl = velocityXPID.Update(Time.fixedDeltaTime, velocityError.x);
            velocityZControl = velocityZPID.Update(Time.fixedDeltaTime, -velocityError.z);
            Debug.Log($"X:{velocityError.x:0.00} | Z:{velocityError.z:0.00}");
        }

        Vector3 translationalControl = new Vector3(-velocityZControl, 0, -velocityXControl);
        //Debug.Log($"X:{translationalControl.x:0.00} | Z:{translationalControl.z:0.00}");

        return translationalControl;
    }

    private Vector3 CalculateVelocityError()
    {
        Rigidbody helicopterBody = Helicopter.GetComponent<Rigidbody>();
        Vector3 velocityLocal = Helicopter.transform.InverseTransformDirection(helicopterBody.linearVelocity);
        Debug.Log($"X:{velocityLocal.x:0.00} | Z:{velocityLocal.z:0.00}");

        return velocityLocal;
    }

    private Vector3 CalculatePositionError(Vector3 targetPosition)
    {
        Vector3 positionOffsetLocal = Helicopter.transform.InverseTransformDirection(targetPosition - Helicopter.transform.position);
        Debug.DrawLine(Helicopter.transform.position, targetPosition);
        return positionOffsetLocal;
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
