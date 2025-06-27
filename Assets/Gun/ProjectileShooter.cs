using UnityEngine;

[CreateAssetMenu(fileName = "Shooter", menuName = "Gun/Shooter/Projectile")]
public class ProjectileShooter : Shooter
{
    public override void Shoot(Transform muzzle)
    {
        base.Shoot(muzzle);
        for (int i = 0; i < bulletsPerShot; i++)
        {
            GameObject currentBullet = Instantiate(bullet, muzzle.position, muzzle.rotation);

            Bullet bulletScript = currentBullet.GetComponent<Bullet>();
            bulletScript.shooter = this;

            bulletScript.dir = muzzle.transform.forward;

            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(muzzle.forward * bulletVelocity, ForceMode.Impulse);
        }

    }
}
