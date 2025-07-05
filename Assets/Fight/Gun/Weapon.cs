using UnityEngine;

public class Weapon : NpcComponent
{
    [SerializeField]
    Shooter shooter;
    [SerializeField]
    FireMode fireMode;

    [SerializeField]
    Transform muzzle;

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
        fireMode.OnTriggerPulled();
    }

    public void ReleaseTrigger()
    {
        fireMode.OnTriggerReleased();
    }

    public void Update()
    {
        if (fireMode.CanFire())
        {
            shooter.Shoot(muzzle);
            fireMode.OnFired();
        }
    }
}
