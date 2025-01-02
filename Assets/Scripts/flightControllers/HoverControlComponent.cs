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
        pitchPID.Kp = 1f;
        pitchPID.Kd = 500f;
        pitchPID.Ki = 0f;

        rollPID = new PID();
        rollPID.Kp = 1f;
        rollPID.Kd = 500f;
        rollPID.Ki = 0f;

        yawPID = new PID();
        yawPID.Kp = 0.2f;
        yawPID.Kd = 400f;
        yawPID.Ki = 0f;
        yawPID.IntegralLimit = 1f;

        altitudePID = new PID();
        altitudePID.Kp = 0.5f;
        altitudePID.Kd = 750f;
        altitudePID.Ki = 0.2f;
        altitudePID.IntegralLimit = 5f;

        velocityXPID = new PID();
        velocityXPID.Kp = 3f;
        velocityXPID.Kd = 100f;
        velocityXPID.Ki = 30f;
        velocityXPID.IntegralLimit = 0.15f;

        velocityZPID = new PID();
        velocityZPID.Kp = 3f;
        velocityZPID.Kd = 100f;
        velocityZPID.Ki = 30f;
        velocityZPID.IntegralLimit = 0.15f;

        //roll axis
        positionXPID = new PID();
        positionXPID.Kp = 0.25f;
        positionXPID.Kd = 10f;
        positionXPID.Ki = 0.01f;
        positionXPID.IntegralLimit = 0.1f;

        //pitch axis
        positionZPID = new PID();
        positionZPID.Kp = 0.25f;
        positionZPID.Kd = 10f;
        positionZPID.Ki = 0.01f;
        positionZPID.IntegralLimit = 0.1f;
    }

    public void Update()
    {
        Vector3 attitudeError = CalculateAttitudeControls();
        float pitchControl = pitchPID.Update(Time.fixedDeltaTime, attitudeError.x);
        pitchControl = Mathf.Clamp(pitchControl, -1, 1);

        float rollControl = rollPID.Update(Time.fixedDeltaTime, attitudeError.y);
        rollControl = Mathf.Clamp(rollControl, -1, 1);

        float yawControl = yawPID.Update(Time.fixedDeltaTime, attitudeError.z);
        yawControl = Mathf.Clamp(yawControl, -1, 1);

        float altitudeControl = altitudePID.Update(Time.fixedDeltaTime, Helicopter.transform.position.y);
        altitudeControl = Mathf.Clamp(altitudeControl, 0.1f, 1);

        //Debug.Log($"Pitch:{pitchControl:0.00} | Roll:{rollControl:0.00} | Yaw:{yawControl:0.00} | Alt:{altitudeControl:0.00}");

        FlightController.ApplyPitch(pitchControl);
        FlightController.ApplyRoll(rollControl);
        FlightController.ApplyYaw(yawControl);
        FlightController.ApplyLift(altitudeControl);

    }

    private Vector3 CalculateAttitudeControls()
    {
        Vector3 velocityControl = CalculateVelocityControl();

        Quaternion targetAttitude = Quaternion.FromToRotation(Helicopter.transform.up, Vector3.up) * Helicopter.transform.rotation;
        //targetAttitude *= Quaternion.Euler(-TargetPitch, 0, -TargetRoll);
        targetAttitude *= Quaternion.Euler(velocityControl.x, 0, velocityControl.z);
        targetAttitude = targetAttitude.normalized;

        Target.transform.position = Helicopter.transform.position;
        Target.transform.rotation = targetAttitude;

        float pitchError = Vector3.SignedAngle(Helicopter.transform.forward, Target.transform.forward, Helicopter.transform.right);
        float rollError = Vector3.SignedAngle(Helicopter.transform.right, Target.transform.right, Helicopter.transform.forward);

        Vector3 targetHeadingXZ = TargetHeading;
        targetHeadingXZ.y = 0;
        Vector3 currentHeadingXZ = Helicopter.transform.forward;
        currentHeadingXZ.y = 0;

        Quaternion targetHeadingRotation = Quaternion.LookRotation(targetHeadingXZ, Vector3.up);
        Quaternion currentHeadingRotation = Quaternion.LookRotation(currentHeadingXZ, Vector3.up);

        float yawSign = -Mathf.Sign(Vector3.SignedAngle(currentHeadingXZ, targetHeadingXZ, Vector3.up));
        float yawError = Quaternion.Angle(currentHeadingRotation, targetHeadingRotation);
        yawError *= yawSign;

        //Debug.Log($"Pitch:{pitchError:0.00} | Roll:{rollError:0.00} | Yaw:{yawError:0.00}");
        DebugDrawTransform();

        return new Vector3(pitchError, rollError, yawError);
    }

    private Vector3 CalculateVelocityControl()
    {
        Vector3 velocityError = CalculateVelocityError();
        float velocityXControl = 0;
        float velocityZControl = 0;
        if (TargetPosition != null)
        {
            Vector3 positionError = CalculatePositionError(TargetPosition.Value);
            float positionXControl = positionXPID.Update(Time.fixedDeltaTime, positionError.x);
            float positionZControl = positionZPID.Update(Time.fixedDeltaTime, -positionError.z);

            velocityXPID.Target = -positionXControl;
            velocityZPID.Target = -positionZControl;

            velocityXControl = velocityXPID.Update(Time.fixedDeltaTime, velocityError.x);
            velocityZControl = velocityZPID.Update(Time.fixedDeltaTime, -velocityError.z);
        }
        else
        {
            velocityXControl = velocityXPID.Update(Time.fixedDeltaTime, velocityError.x);
            velocityZControl = velocityZPID.Update(Time.fixedDeltaTime, -velocityError.z);
            //Debug.Log($"X:{velocityError.x:0.00} | Z:{velocityError.z:0.00}");
        }

        Vector3 translationalControl = new Vector3(-velocityZControl, 0, -velocityXControl);
        //Debug.Log($"X:{translationalControl.x:0.00} | Z:{translationalControl.z:0.00}");

        return translationalControl;
    }

    private Vector3 CalculateVelocityError()
    {
        Rigidbody helicopterBody = Helicopter.GetComponent<Rigidbody>();
        Vector3 velocityLocal = Helicopter.transform.InverseTransformDirection(helicopterBody.linearVelocity);
        //Debug.Log($"X:{velocityLocal.x:0.00} | Z:{velocityLocal.z:0.00}");

        return velocityLocal;
    }

    private Vector3 CalculatePositionError(Vector3 targetPosition)
    {
        Vector3 positionOffsetLocal = Helicopter.transform.InverseTransformDirection(targetPosition - Helicopter.transform.position);
        float distance = positionOffsetLocal.magnitude;
        float maxDistance = 100;
        if (distance != 0 && distance > maxDistance)
        {
            positionOffsetLocal *= maxDistance / distance;
        }
        //Debug.Log($"X:{positionOffsetLocal.x:0.00} | Z:{positionOffsetLocal.z:0.00}");
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
        Debug.DrawLine(pos, pos + TargetHeading *  magnitude, Color.white);
    }
}
