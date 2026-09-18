using System.Collections;
using UnityEngine;

public class AiFireDirector : MonoBehaviour
{
    private Coroutine shootCoroutine;
    private Dog dog;

    [Header("Burst Settings")]
    public float burstPauseMin = 3.0f;
    public float burstPauseMax = 6.5f;
    public int minShotsPerBurst = 1;
    public int maxShotsPerBurst = 9;

    [Header("Accuracy Bloom")]
    public float maxBloom = 1.5f; // First shot misses
    public float minBloom = 0.2f; // Last shot is accurate

    public void StartDirector(Dog dog)
    {
        this.dog = dog;
        if (shootCoroutine == null)
            shootCoroutine = StartCoroutine(ShootingRoutine());
    }

    public void StopDirector()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (true)
        {
            if (!dog.npc.canShoot || !dog.targeting.TargetInSight)
            {
                yield return null;
                continue;
            }

            // 1. DETERMINE BURST SIZE FIRST
            int shots = dog.npc.isPanicking ? 30 : Random.Range(minShotsPerBurst, maxShotsPerBurst + 1);

            // 2. FIRE THE WEAPON
            for (int i = 0; i < shots; i++)
            {
                if (!dog.npc.canShoot || !dog.targeting.TargetInSight) break;

                if (shots > maxShotsPerBurst && !dog.npc.isPanicking && i >= 5)
                {
                    break;
                }

                float t = (shots > 1) ? (float)i / (shots - 1) : 1f;
                float directorBloom = Mathf.Lerp(maxBloom, minBloom, t);

                yield return StartCoroutine(FireBulletCoroutine(directorBloom));

                // Wait for the gun's mechanical fire rate
                yield return new WaitForSeconds(dog.npc.weapon.fireMode.rate);
            }

            // 3. THE HUMAN BREATH (Moved to the end!)
            // Now the AI rests AFTER shooting the burst, not before.
            float pause = dog.npc.isPanicking ? 1.0f : Random.Range(burstPauseMin, burstPauseMax);
            yield return new WaitForSeconds(pause);
        }
    }

    private IEnumerator FireBulletCoroutine(float directorBloom)
    {
        // This respects physics (if moving fast) but forces the 'Director Miss'.
        dog.npc.currentBloom = Mathf.Max(dog.npc.currentBloom, directorBloom);
        // 1. Pull the trigger
        dog.npc.TryShoot?.Invoke();

        // 2. WAIT one frame! This allows Weapon.Update() to see 'isFiring = true'
        yield return null;

        // 3. Release the trigger
        dog.npc.NotShoot?.Invoke();
    }
}