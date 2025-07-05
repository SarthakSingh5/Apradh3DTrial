using UnityEngine;


public class FireMode : ScriptableObject
{
    public float rate = 0.3f; // Time in seconds between shots
    public float nextFireTime = 0f; // Time when the next shot can be fired

    public virtual void OnEnable()
    {
        nextFireTime = 0f; // Reset next fire time when the fire mode is enabled
    }

    public virtual void OnTriggerPulled()
    {
    }

    public virtual void OnTriggerReleased()
    {
    }

    public virtual bool CanFire()
    {
        return true; // Default implementation allows firing
    }
    
    public virtual void OnFired()
    {
        // Implement logic to handle after firing, like resetting cooldown
    }
}
