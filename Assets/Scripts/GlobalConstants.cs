using UnityEditor.PackageManager;
using UnityEngine;

// Intended to store global truths to be accessed by functions

public static class GlobalConstants
{
    public enum rotorConfigurations{Conventional,Tandem,Coax,SideBySide,Quad};

    static public FlightController returnFlightControllerType(GameObject helicopter){
        if (helicopter.CompareTag("tandem"))
        {
            return helicopter.GetComponent<TandemFlightController>();
        }
        else if (helicopter.CompareTag("conventional"))
        {
            return helicopter.GetComponent<ConventionalFlightController>();
        }
        else if (helicopter.CompareTag("coax"))
        {
            return helicopter.GetComponent<CoaxFlightController>();
        }
        else if (helicopter.CompareTag("quad"))
        {
            return helicopter.GetComponent<QuadFlightController>();
        }
        else if (helicopter.CompareTag("sidebyside"))
        {
            return helicopter.GetComponent<SideBySideFlightController>();
        }
        else{
            Debug.LogError("Tag not recognized as supported rotor configuration. Tag givem: " + helicopter.tag);
            return helicopter.GetComponent<SideBySideFlightController>();
        }
    }

}

