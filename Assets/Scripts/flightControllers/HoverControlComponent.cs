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

    public HoverControlComponent(FlightController flightController)
    {
        FlightController = flightController;
        Target = new GameObject();

        pitchPID = new PID();
        pitchPID.Ki = 0f;

        rollPID = new PID();
        rollPID.Ki = 0f;

        yawPID = new PID();
        yawPID.Kp = 0.1f;
        yawPID.Kd = 0.1f;
        yawPID.Ki = 0f;
        yawPID.IntegralLimit = 0.1f;

        altitudePID = new PID();
        altitudePID.Kp = 1.5f;
        altitudePID.Kd = 80f;
        altitudePID.Ki = 0.1f;
        altitudePID.IntegralLimit = 10f;
    }

    public void Update()
    {
        altitudePID.Kp = 1.5f;
        altitudePID.Kd = 80f;
        altitudePID.Ki = 0.1f;


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
        Quaternion targetAttitude = Quaternion.FromToRotation(Helicopter.transform.up, Vector3.up) * Helicopter.transform.rotation;
        Quaternion targetOrientation = Quaternion.FromToRotation(Helicopter.transform.forward, TargetHeading) * targetAttitude;

        targetAttitude = targetAttitude.normalized;
        targetOrientation = targetOrientation.normalized;

        Target.transform.position = Helicopter.transform.position;
        Target.transform.rotation = targetAttitude;

        float pitchError = Vector3.SignedAngle(Helicopter.transform.forward, Target.transform.forward, Helicopter.transform.right);
        float rollError = Vector3.SignedAngle(Helicopter.transform.right, Target.transform.right, Helicopter.transform.forward);

        float yawSign = -Mathf.Sign(Vector3.SignedAngle(Target.transform.forward, TargetHeading, Vector3.up));
        float yawError = Quaternion.Angle(targetAttitude, targetOrientation);
        yawError *= yawSign;

        //Debug.Log($"Yaw:{yawError:0.00}");
        //Debug.Log($"Pitch:{pitchError:0.00} | Roll:{rollError:0.00}");
        //DebugDrawTransform();

        return new Vector3(pitchError, rollError, yawError);
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
