using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Shooter", menuName = "Gun/Shooter/HitScan")]
public class HitscanShooter : Shooter
{
	[Header("Tactical Pressure Settings")]
	[SerializeField] private float suppressionRadius = 3.5f;
	[SerializeField] private LayerMask npcLayerMask;

	public override void Shoot(Transform muzzle, float bloom, Transform ownerTransform)
	{
		base.Shoot(muzzle, bloom, ownerTransform);
		SpawnHitScanProjectile(muzzle, bloom, ownerTransform);
	}

	Vector3 ComputeShootingDirection(Transform muzzle, float bloom)
	{
		// Upgraded to use dynamic bloom spread matching your ProjectileShooter
		Vector2 spread = Random.insideUnitCircle * bloom * spreadMultiplier;
		Quaternion spreadRotation = Quaternion.Euler(spread.x, spread.y, 0);
		return muzzle.rotation * spreadRotation * Vector3.forward;
	}

	void ApplyHitImpulse(RaycastHit hit, Vector3 direction)
	{
		if (hit.rigidbody != null)
		{
			hit.rigidbody.AddForceAtPosition(direction * HitImpulse, hit.point, ForceMode.Impulse);
		}
	}

	void SpawnHitScanProjectile(Transform muzzle, float bloom, Transform ownerTransform)
	{
		Vector3 shootDirection = ComputeShootingDirection(muzzle, bloom);
		Ray ray = new Ray(muzzle.position, shootDirection);
		Vector3 endPoint = ray.origin + ray.direction * MaxRange;

		RaycastHit[] hits = Physics.RaycastAll(ray, MaxRange, HitMask, QueryTriggerInteraction.Ignore);
		System.Array.Sort(hits, (a, b) => { return a.distance.CompareTo(b.distance); });

		foreach (RaycastHit hit in hits)
		{
			IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

			if (damageable != null)
			{
				float damage = Damage + Random.Range(0f, DamageRandom);
				damageable.Damage(damage);
				ApplyHitImpulse(hit, ray.direction);
				
				endPoint = hit.point;
				break;
			}
			else
			{
				ApplyHitImpulse(hit, ray.direction);
				endPoint = hit.point;

				// --- IMPACT PRESSURE ---
				Collider[] impactSuppression = Physics.OverlapSphere(endPoint, suppressionRadius, npcLayerMask);
				foreach (Collider col in impactSuppression)
				{
					if (col.TryGetComponent(out Npc npc))
					{
						if (ownerTransform != null && ownerTransform.IsChildOf(npc.transform)) continue;

						float distance = Vector3.Distance(endPoint, npc.transform.position);
						float intensity = 1f - (distance / suppressionRadius);
						npc.AddPressure(intensity * 0.4f);
					}
				}
				break;
			}
		}

		// --- WHIZ-BY PRESSURE ---
		// Draws one instantaneous capsule from the barrel to the final hit point
		List<Npc> suppressedNpcs = new List<Npc>();
		Collider[] nearMisses = Physics.OverlapCapsule(muzzle.position, endPoint, suppressionRadius, npcLayerMask);
		
		foreach (Collider col in nearMisses)
		{
			if (col.TryGetComponent(out Npc npc))
			{
				if (ownerTransform != null && ownerTransform.IsChildOf(npc.transform)) continue;

				if (!suppressedNpcs.Contains(npc))
				{
					npc.AddPressure(0.15f);
					suppressedNpcs.Add(npc);
				}
			}
		}

		MonoBehaviourHelper.RunCoroutine(CR_SpawnTracer(muzzle.position, endPoint));
	}

	IEnumerator CR_SpawnTracer(Vector3 start, Vector3 end)
	{
		LineRenderer tracer = Instantiate(tracerPrefab, start, Quaternion.identity);

		float distance = Vector3.Distance(start, end);
		float elapsedTime = 0f;

		while (elapsedTime < tracerLifetime)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime * tracerSpeed / distance);
			Vector3 currentPoint = Vector3.Lerp(start, end, t);
			
			tracer.SetPosition(0, start);
			tracer.SetPosition(1, currentPoint);

			yield return null;
		}

		tracer.SetPosition(1, end);
		Destroy(tracer.gameObject, tracerLifetime);
	}
}