using UnityEngine;

public abstract class FlightController : MonoBehaviour
{
    // Method to apply lift to the rotorcraft
    public abstract void ApplyLift(float liftInput);

    // Method to apply roll to the rotorcraft
    public abstract void ApplyRoll(float rollInput);

    // Method to apply pitch to the rotorcraft
    public abstract void ApplyPitch(float pitchInput);

    // Method to apply yaw to the rotorcraft
    public abstract void ApplyYaw(float yawInput);

    public abstract void ApplyThrust(float thrustInput);
}
