using UnityEngine;

[CreateAssetMenu(fileName = "FireMode", menuName = "Gun/FireMode/Semi")]
public class FireModeSemi : FireMode
{
    bool hasFired;

    public override void OnTriggerPulled()
    {
        hasFired = false;
    }

    public override void OnTriggerReleased()
    {
        hasFired = true;
    }

    public override bool CanFire()
    {
        if (hasFired == false && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + rate; // Set the next fire time based on the rate
            return true;
        }

        return false;
    }

    public override void OnFired()
    {
    }
}
