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

    private float maxOutputObserved = 0f;

    public float GetControlOutput(float error)
    {

        integral += error * Time.fixedDeltaTime; // Integrate error over time
        float derivative = (error - perviousError) / Time.fixedDeltaTime; // Calculate rate of change

        derivative = Mathf.Lerp(lastDerivative, derivative, 0.1f);  // Smooth it a bit
        lastDerivative = derivative; // Update the last derivative for next time

        // PID formula: output = Kp * error + Ki * integral + Kd * derivative
        float output = Kp * error + Ki * integral + Kd * derivative;

        perviousError = error; // Update previous error
   
        if (Mathf.Abs(output)>Mathf.Abs(maxOutputObserved)){
            maxOutputObserved=Mathf.Abs(output);
        }

        output=output/maxOutputObserved;

        if (float.IsNaN(output)){
            output=0;
        }
        
        //Debug.Log($"Error: {error}, Proportional: {Kp * error}, Integral: {Ki * integral}, Derivative: {Kd * derivative}, Output: {output}");
        return output;
    }
}
