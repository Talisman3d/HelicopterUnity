using UnityEngine;

// Abstract Flight Controller class, so that I can separate a player input or AI input from flight control interpretation
public abstract class FlightController : MonoBehaviour
{
    public abstract void ApplyLift(float liftInput);

    public abstract void ApplyRoll(float rollInput);

    public abstract void ApplyPitch(float pitchInput);

    public abstract void ApplyYaw(float yawInput);

    public abstract void ApplyThrust(float thrustInput);
}
