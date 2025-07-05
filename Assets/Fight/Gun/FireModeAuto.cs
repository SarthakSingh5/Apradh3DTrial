using UnityEngine;

[CreateAssetMenu(fileName = "FireMode", menuName = "Gun/FireMode/Auto")]
public class FireModeAuto : FireMode
{
    bool isFiring;

    public override void OnTriggerPulled()
    {
        isFiring = true; // Start firing when the trigger is pulled
    }

    public override void OnTriggerReleased()
    {
        isFiring = false; // Stop firing when the trigger is released
    }

    public override bool CanFire()
    {
        // Allow firing if the trigger is pulled and enough time has passed since the last shot
        return isFiring && Time.time >= nextFireTime;
    }

    public override void OnFired()
    {
        nextFireTime = Time.time + rate; // Set the next fire time based on the rate
    }
}
