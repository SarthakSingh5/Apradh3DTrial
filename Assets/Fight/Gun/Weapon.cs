using UnityEngine;

public class Weapon : NpcComponent
{
    [SerializeField]
    Shooter shooter;
    [SerializeField]
    FireMode fireMode;

    [SerializeField]
    Transform muzzle;

    // Each AK-47 in the scene has its own private memory now.
    private FireModeState state = new FireModeState();

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 1.0 makes it fully 3D
    }


    public override void SetNpc(Npc npc)
    {
        if (this.npc != null)
        {
            this.npc.TryShoot -= TryShoot;
            this.npc.NotShoot -= NotShoot;
            this.npc.LookAt -= OnLookAt;
        }

        base.SetNpc(npc);

        if (npc != null)
        {
            npc.TryShoot += TryShoot;
            npc.NotShoot += NotShoot;
            npc.LookAt += OnLookAt;

        }
    }

    void TryShoot()
    {
        PullTrigger();
    }

    void NotShoot()
    {
        ReleaseTrigger();
    }



    void OnLookAt(Vector3 target)
    {
        // we orient the muzzle towards the target, like they did in games made in the 90s.
        // if you want realistic aiming, you should use animation rigging
        // Code Monkey has a great tutorial on that on Youtube.
        muzzle.forward = target - muzzle.position;
    }

    public void PullTrigger()
    {
        fireMode.OnTriggerPulled(state);
    }

    public void ReleaseTrigger()
    {
        fireMode.OnTriggerReleased(state);
    }

    public void Update()
    {
        if (fireMode.CanFire(state))
        {
            shooter.Shoot(muzzle);
            fireMode.OnFired(state);
            PlayShotSound();
        }
    }

    void PlayShotSound()
    {
        if (shooter.shotSound != null)
        {
            // Randomize pitch slightly for a better feel
            audioSource.pitch = 1.0f + Random.Range(-shooter.pitchRandomness, shooter.pitchRandomness);
            audioSource.PlayOneShot(shooter.shotSound, shooter.volume);
        }
    }
}
