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

            // 1. THE HUMAN BREATH
            // If panicking, the AI is aggressive and pauses less.
            float pause = dog.npc.isPanicking ? 1.0f : Random.Range(burstPauseMin, burstPauseMax);
            yield return new WaitForSeconds(pause);

            // 2. THE BURST SIZE
            // If panicking, dump 30 rounds. If calm, use your 2-5 or 9-15 range.
            int shots = dog.npc.isPanicking ? 30 : Random.Range(minShotsPerBurst, maxShotsPerBurst + 1);

            for (int i = 0; i < shots; i++)
            {
                if (!dog.npc.canShoot || !dog.targeting.TargetInSight) break;

                // --- THE DYNAMIC EXIT RULE ---
                // If we started as a PANIC burst (30 shots) but the target got away...
                // AND we have already fired at least 5 shots...
                // STOP firing and go back to the 'Big Breath' pause.
                if (shots > maxShotsPerBurst && !dog.npc.isPanicking && i >= 5)
                {
                    break;
                }

                // 3. YOUR EXACT BLOOM LOGIC
                float t = (shots > 1) ? (float)i / (shots - 1) : 1f;
                float directorBloom = Mathf.Lerp(maxBloom, minBloom, t);

                yield return StartCoroutine(FireBulletCoroutine(directorBloom));

                // 4. THE WEAPON RATE
                // No extra random delay here! Just the mechanical speed of the gun.
                yield return new WaitForSeconds(dog.npc.weapon.fireMode.rate);
            }
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