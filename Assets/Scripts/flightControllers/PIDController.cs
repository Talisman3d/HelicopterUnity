using System;
using UnityEngine;

// PID controller code I grabbed from the internet
// Added some safeties to clamp output from -1 to 1 and handle NaN
public class PIDController
{
    public float Kp; // Proportional Gain
    public float Ki; // Integral Gain
    public float Kd; // Derivative Gain

    private float perviousError = 0f;
    private float integral = 0f;
    private float lastDerivative = 0f;

    Boolean saturated = false;
    //https://www.dmi.unict.it/santoro/teaching/sr/slides/PIDSaturation.pdf

    public float GetControlOutput(float error)
    {

        if (!saturated){
            integral += error * Time.fixedDeltaTime; // Integrate error over time
        }
        else{
            integral = 0;
        }
        

        float derivative = (error - perviousError) / Time.fixedDeltaTime; // Calculate rate of change

        derivative = Mathf.Lerp(lastDerivative, derivative, 0.1f);  // Smooth it a bit
        lastDerivative = derivative; // Update the last derivative for next time

        // PID formula: output = Kp * error + Ki * integral + Kd * derivative
        float output = Kp * error + Ki * integral + Kd * derivative;

        perviousError = error; // Update previous error

        if (float.IsNaN(output)){
            output=0;
        }
        
        // Clamp output at -1 and 1
        // In control terms, this is called saturation (as I'm learning)
        if (output>1){
            output=1;
            saturated=true;
        }
        else if(output<-1){
            output=-1;
            saturated = true;
        }
        else{
            saturated=false;
        }

        //Debug.Log($"Error: {error}, Proportional: {Kp * error}, Integral: {Ki * integral}, Derivative: {Kd * derivative}, Output: {output}");
        return output;
    }
}
