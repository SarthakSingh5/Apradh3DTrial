using System.Collections;
using UnityEngine;

public class AiFireDirector : MonoBehaviour
{
    private Coroutine shootCoroutine;
    private Dog dog;

    [Header("Burst Settings")]
    public float burstPauseMin = 1.5f;
    public float burstPauseMax = 3.0f;
    public int minShotsPerBurst = 2;
    public int maxShotsPerBurst = 5;

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
            // 1. GATEKEEPER: Only shoot if the variable allows it AND target is seen
            if (!dog.npc.canShoot || !dog.targeting.TargetInSight)
            {
                yield return null;
                continue;
            }

            // 2. PAUSE BETWEEN BURSTS (The player's window to move)
            yield return new WaitForSeconds(Random.Range(burstPauseMin, burstPauseMax));

            // 3. START BURST
            int shots = Random.Range(minShotsPerBurst, maxShotsPerBurst + 1);
            for (int i = 0; i < shots; i++)
            {
                // Stop mid-burst if we lose permission (e.g. ducked into cover or target lost)
                if (!dog.npc.canShoot || !dog.targeting.TargetInSight) break;

                // Accuracy gets better as the burst progresses (RDR2 style)
                float t = (shots > 1) ? (float)i / (shots - 1) : 1f;
                float currentBloom = Mathf.Lerp(maxBloom, minBloom, t);

                FireBullet(currentBloom);

                // HUMAN CADENCE: Random time between bullets within the burst
                yield return new WaitForSeconds(Random.Range(0.12f, 0.25f));
            }
        }
    }

    private void FireBullet(float bloom)
    {
        // Directional log to keep things clean
        Debug.Log($"<color=orange>Director:</color> Requesting shot with {bloom} bloom");
        
        // This triggers the mechanical shooter on the weapon
        dog.npc.TryShoot?.Invoke(); 
    }
}