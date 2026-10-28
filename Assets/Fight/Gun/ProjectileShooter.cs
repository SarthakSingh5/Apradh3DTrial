using UnityEngine;

[CreateAssetMenu(fileName = "Shooter", menuName = "Gun/Shooter/Projectile")]
public class ProjectileShooter : Shooter
{
    // NEW: Added ownerTransform to the signature to match the base class
    public override void Shoot(Transform muzzle, float bloom, Transform ownerTransform)
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            // 1. Calculate the Random Spread based on the passed bloom
            Vector2 spread = Random.insideUnitCircle * bloom * spreadMultiplier;
            
            // 2. Convert that to a Rotation (Pitch and Yaw)
            Quaternion spreadRotation = Quaternion.Euler(spread.x, spread.y, 0);
            
            // 3. Combine with Muzzle Rotation
            Quaternion finalRotation = muzzle.rotation * spreadRotation;

            // 4. Spawn Bullet
            GameObject currentBullet = Instantiate(bullet, muzzle.position, finalRotation);

            Bullet bulletScript = currentBullet.GetComponent<Bullet>();
            bulletScript.shooter = this;
            bulletScript.dir = finalRotation * Vector3.forward;
            
            // NEW: Pass the transform of whoever shot the gun to the bullet
            bulletScript.ownerTransform = ownerTransform;

            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(bulletScript.dir * bulletVelocity, ForceMode.Impulse);
        }
    }
}