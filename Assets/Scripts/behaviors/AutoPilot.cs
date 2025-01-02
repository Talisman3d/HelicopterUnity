using UnityEngine;

// Goal of this class is to interface with HoverModeControl
// Handle if desired to hover or move towards target
// And be applicable to player controller and AI
public class AutoPilot : MonoBehaviour
{
    GameObject helicopter;

    public bool isAutoPilotEnabled = false;
    public FlightController FlightController { get; protected set; }
    public HoverControlComponent HoverControl { get; protected set; }
   
    void Start()
    {
        helicopter = transform.GetChild(0).gameObject;

        if (helicopter.CompareTag("tandem"))
        {
            FlightController = helicopter.GetComponent<TandemFlightController>();
        }
        else if (helicopter.CompareTag("conventional"))
        {
            FlightController = helicopter.GetComponent<ConventionalFlightController>();
        }
        else if (helicopter.CompareTag("coax"))
        {
            FlightController = helicopter.GetComponent<CoaxFlightController>();
        }
        else if (helicopter.CompareTag("quad"))
        {
            FlightController = helicopter.GetComponent<QuadFlightController>();
        }
        else if (helicopter.CompareTag("sidebyside"))
        {
            FlightController = helicopter.GetComponent<SideBySideFlightController>();
        }
        HoverControl = new HoverControlComponent(FlightController);
        // Assuming helicopter type is only child of player/AI
        
        HoverInPlace();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAutoPilotEnabled){
            HoverControl.Update();
        }
        
    }

    public void MoveToTarget(GameObject target)
    {
        HoverControl.TargetPosition = target.transform.position;
        HoverControl.TargetAltitude = target.transform.position.y;
        HoverControl.TargetHeading = (target.transform.position - helicopter.transform.position).normalized;
    }

    public void HoverInPlace(){
        HoverControl.TargetPosition = helicopter.transform.position;
        HoverControl.TargetAltitude = helicopter.transform.position.y+0.2f;
        HoverControl.TargetHeading = helicopter.transform.forward;

    }
    
}
