using System;
using Unity;
using UnityEngine;

public class PID
{
    public float Kp = 1;
    public float Kd = 1;
    public float Ki = 1;
    private float integral = 0;
    public float PreviousState { get; protected set; }
    public float Target { get; set; }
    public float IntegralLimit { get; set; } = 0;

    public float Update(float dt, float newState)
    {
        float output = 0;

        float error = Target - newState;

        float deltaError = 0;
        if (dt > 0)
        {
            integral += error * dt;
            deltaError = (error - (Target - PreviousState)) * dt;
        }
        if (IntegralLimit != 0)
        {
            integral = Mathf.Clamp(integral, -IntegralLimit, IntegralLimit);
        }

        float p = error * Kp;
        float i = integral * Ki;
        float d = deltaError * Kd;

        output = p + i + d;
        PreviousState = newState;
        return output;
    }

}

