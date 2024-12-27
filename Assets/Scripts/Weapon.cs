using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract void FirePrimary();

    // Method to apply roll to the rotorcraft
    public abstract void FireSecondary();
}
