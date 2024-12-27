using UnityEngine;

// WIP
// This will become parent class to be able to handle any weapon slotted into primary or secondary slots
public abstract class Weapon : MonoBehaviour
{
    public abstract void FirePrimary();

    // Method to apply roll to the rotorcraft
    public abstract void FireSecondary();
}
