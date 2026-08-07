// using System.Collections;
// using UnityEngine;
// using UnityEngine.AI;
// using System.Collections.Generic;

// public class AiCoverMovement : MonoBehaviour
// {
//     public LayerMask HidableLayers;
//     public EnemyLineOfSightChecker LineOfSightChecker;

//     [Range(-1, 1)]
//     [Tooltip("Lower is a better hiding spot")]
//     public float HideSensitivity = 0;

//     [Range(1, 10)]
//     public float MinTargetDistance = 5f;

//     [Range(0, 5f)]
//     public float MinObstacleHeight = 1.25f;

//     [Range(0, 5f)]
//     public float Updatefrequency = 5f; // seconds
//     [Range(5f, 50f)]
//     public float MaxTargetDistance = 15f; // if target moves beyond this, stop hiding

//     private Coroutine peekCoroutine = null;
//     private Coroutine movementCoroutine = null;
//     private Collider[] Colliders = new Collider[10]; // more is less performant, but more options

//     [Header("Peeking Settings")]
//     public float PeekOffsetDistance = 1f;
//     public float TimeBetweenPeeks = 3f;
//     public float PeekDuration = 1f;

//     private Vector3 lastHidePosition;
//     public bool isPeeking = false;


//     public void StartHiding(Dog dog)
//     {
//         StopHiding(dog);
//         movementCoroutine = StartCoroutine(Hide(dog));
//         peekCoroutine = StartCoroutine(PeekAtTargetRoutine(dog));

//     }


//     public void StopHiding(Dog dog)
//     {
//         if (movementCoroutine != null)
//         {
//             StopCoroutine(movementCoroutine);
//             movementCoroutine = null;
//         }
//         if (peekCoroutine != null)
//         {
//             StopCoroutine(peekCoroutine);
//             peekCoroutine = null;
//         }
//         isPeeking = false;
//         dog.npc.inCover = false;

//     }

//     public bool HasAnyCover(Vector3 TargetPosition)
//     {
//         int hits = Physics.OverlapSphereNonAlloc(transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

//         int validHits = 0;

//         for (int i = 0; i < hits; i++)
//         {
//             if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) >= MinTargetDistance &&
//                 Colliders[i].bounds.size.y >= MinObstacleHeight)
//             {
//                 validHits++;
//             }
//         }

//         return validHits > 0;
//     }


//     private IEnumerator Hide(Dog dog)
//     {
//         if (peekCoroutine != null)
//         {
//             StopCoroutine(peekCoroutine);
//             peekCoroutine = null;
//         }
//         WaitForSeconds Wait = new WaitForSeconds(Updatefrequency);


//         while (true)
//         {
//             Vector3 TargetPosition = dog.targeting.TargetPosition;
//             for (int i = 0; i < Colliders.Length; i++)
//             {
//                 Colliders[i] = null;
//             }

//             int hits = Physics.OverlapSphereNonAlloc(dog.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

//             int hitReduction = 0;
//             for (int i = 0; i < hits; i++)
//             {
//                 if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) < MinTargetDistance || Colliders[i].bounds.size.y < MinObstacleHeight)
//                 {
//                     Colliders[i] = null;
//                     hitReduction++;
//                 }
//             }

//             hits -= hitReduction;

//             System.Array.Sort(Colliders, ColliderArraySortComparer);

//             for (int i = 0; i < hits; i++)
//             {
//                 if (Colliders[i].bounds.size.y >= 3)
//                 {
//                     List<Vector3> candidatePoints = GenerateCoverPoints(Colliders[i]);

//                     foreach (var point in candidatePoints)
//                     {
//                         if (NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, dog.agent.areaMask))
//                         {
//                             if (!NavMesh.FindClosestEdge(hit.position, out hit, dog.agent.areaMask))
//                             {
//                                 Debug.LogError($"Unable to find edge close to (hit.position)");
//                             }
//                             // Vector3 rotated = Quaternion.Euler(0, -90, 0) * hit.normal;
//                             // Vector3 a = hit.normal.normalized;
//                             // Vector3 b = (TargetPosition - hit.position).normalized;

//                             // float sinTheta = Vector3.Cross(a, b).magnitude;
//                             // float cosTheta = Vector3.Dot(a, b);
//                             // float tanTheta = sinTheta / cosTheta;
//                             if (Vector3.Dot(hit.normal, (TargetPosition - hit.position).normalized) < HideSensitivity)
//                             {

//                                 dog.npc.inCover = false;
//                                 lastHidePosition = hit.position;



//                                 dog.npc.SetDestination?.Invoke(lastHidePosition);

//                                 yield return new WaitUntil(() =>!dog.agent.pathPending && dog.agent.remainingDistance < 0.2f);

//                                 dog.npc.SetAim(false);
//                                 dog.npc.inCover = true;
//                                 Vector3 pos = dog.transform.position;
//                                 pos += hit.normal * 1000.0f;
//                                 pos += Vector3.up * 1.5f;
//                                 dog.npc.LookAt(pos);

//                                 break;

//                             }
//                         }
//                         else
//                         {
//                             Debug.LogError($"Unable to find NavMesh near object (Colliders[{i}].name) at {Colliders[i].transform.position}");
//                         }
//                     }
//                 }
//                 else
//                 {
//                     if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit1, 20f, dog.agent.areaMask))
//                     {
//                         if (!NavMesh.FindClosestEdge(hit1.position, out hit1, dog.agent.areaMask))
//                         {
//                             Debug.LogError($"Unable to find edge close to {hit1.position}");
//                         }

//                         if (Vector3.Dot(hit1.normal, (TargetPosition - hit1.position).normalized) < HideSensitivity)
//                         {
//                             dog.npc.SetDestination?.Invoke(hit1.position);
//                             break;
//                         }
//                         else
//                         {
//                             if (NavMesh.SamplePosition(Colliders[i].transform.position - (TargetPosition - hit1.position).normalized * 2, out NavMeshHit hit2, 20f, dog.agent.areaMask))
//                             {
//                                 if (!NavMesh.FindClosestEdge(hit2.position, out hit2, dog.agent.areaMask))
//                                 {
//                                     Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
//                                 }

//                                 if (Vector3.Dot(hit2.normal, (TargetPosition - hit2.position).normalized) < HideSensitivity)
//                                 {
//                                     dog.npc.SetDestination?.Invoke(hit2.position);
//                                     break;
//                                 }
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
//                     }

//                 }

//             }
//             yield return Wait;

//         }
//     }

//     private int ColliderArraySortComparer(Collider A, Collider B)
//     {
//         if (A == null && B != null)
//         {
//             return 1;
//         }
//         else if (A != null && B == null)
//         {
//             return -1;
//         }
//         else if (A == null && B == null)
//         {
//             return 0;
//         }
//         else
//         {
//             return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
//         }
//     }

//     List<Vector3> GenerateCoverPoints(Collider collider)
//     {
//         List<Vector3> points = new List<Vector3>();
//         Bounds bounds = collider.bounds;
//         Vector3 center = bounds.center;
//         Vector3 extents = bounds.extents;

//         float inset = 0.2f; // distance to move inward from the actual corner

//         // X and Z directions
//         float x = extents.x;
//         float z = extents.z;

//         // 8 face-near-corner points (slightly inward from corners)
//         points.Add(center + new Vector3(-x + inset, 0, z)); // front-left (inward on x)
//         points.Add(center + new Vector3(-x, 0, z - inset)); // front-left (inward on z)

//         points.Add(center + new Vector3(x - inset, 0, z)); // front-right
//         points.Add(center + new Vector3(x, 0, z - inset)); // front-right

//         points.Add(center + new Vector3(-x + inset, 0, -z)); // back-left
//         points.Add(center + new Vector3(-x, 0, -z + inset)); // back-left

//         points.Add(center + new Vector3(x - inset, 0, -z)); // back-right
//         points.Add(center + new Vector3(x, 0, -z + inset)); // back-right

//         return points;
//     }


//     private IEnumerator PeekAtTargetRoutine(Dog dog)
//     {
//         WaitForSeconds waitBetweenPeeks = new WaitForSeconds(TimeBetweenPeeks);
//         WaitForSeconds peekDuration = new WaitForSeconds(PeekDuration);

//         while (true)
//         {
//             Vector3 TargetPosition = dog.targeting.TargetPosition;

//             // Wait until agent is in cover and not moving
//             if (dog.agent.pathPending || Vector3.Distance(transform.position, lastHidePosition) > 0.5f)
//             {
//                 yield return null;
//                 continue;
//             }

//             // Ensure no peeking if state is about to change
//             if (!dog.npc.inCover)
//             {
//                 yield return null;
//                 continue;
//             }


//             Vector3 directionToTarget = (TargetPosition - lastHidePosition).normalized;
//             Vector3 peekPosition = lastHidePosition + directionToTarget * PeekOffsetDistance;

//             if (NavMesh.SamplePosition(peekPosition, out NavMeshHit peekHit, 2f, dog.agent.areaMask))
//             {
//                 dog.npc.SetDestination?.Invoke(peekHit.position);
//                 yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
//                 isPeeking = true;

//                 // Ensure agent stays at peek position for the full duration
//                 yield return peekDuration;
//                 isPeeking = false;

//                 // Return to cover position
//                 dog.npc.SetDestination?.Invoke(lastHidePosition);
//                 yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
//             }
//             else
//             {
//                 Debug.LogWarning($"No valid NavMesh position for peek at {peekPosition}");
//             }

//             // Wait before next peek
//             yield return waitBetweenPeeks;
//         }
//     }

// }








using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AiCoverMovement : MonoBehaviour
{
    public LayerMask HidableLayers; // Make sure this is set to your "Cover" layer!
    public EnemyLineOfSightChecker LineOfSightChecker;

    [Range(1, 10)]
    public float MinTargetDistance = 5f;

    [Range(-1, 1)]
    [Tooltip("Requires the target to be on the opposite side of the wall. Lower = stricter.")]
    public float HideSensitivity = -0.1f;

    [Range(0, 5f)]
    public float Updatefrequency = 5f; 

    [Header("Peeking Settings")]
    public float PeekOffsetDistance = 1f; // How far past the blue node they step out
    public float TimeBetweenPeeks = 3f;
    public float PeekDuration = 1f;

    private Coroutine peekCoroutine = null;
    private Coroutine movementCoroutine = null;
    private Collider[] Colliders = new Collider[10]; 

    private Vector3 safeHidePosition;
    public bool isPeeking = false;
    private CoverSpline currentCover;

    public void StartHiding(Dog dog)
    {
        StopHiding(dog);
        movementCoroutine = StartCoroutine(Hide(dog));
        peekCoroutine = StartCoroutine(PeekAtTargetRoutine(dog));
    }

    public void StopHiding(Dog dog)
    {
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        if (peekCoroutine != null) StopCoroutine(peekCoroutine);
        
        movementCoroutine = null;
        peekCoroutine = null;
        isPeeking = false;
        dog.npc.inCover = false;
        currentCover = null;
    }

    public bool HasAnyCover(Vector3 TargetPosition)
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

        int validHits = 0;

        for (int i = 0; i < hits; i++)
        {
            if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) >= MinTargetDistance)
            {
                validHits++;
            }
        }

        return validHits > 0;
    }

    private IEnumerator Hide(Dog dog)
    {
        WaitForSeconds Wait = new WaitForSeconds(Updatefrequency);

        while (true)
        {
            Vector3 TargetPosition = dog.targeting.TargetPosition;
            int hits = Physics.OverlapSphereNonAlloc(dog.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            CoverSpline bestSpline = null;
            float shortestDist = Mathf.Infinity;
            Vector3 bestSafePoint = Vector3.zero;

            // Find the best spline and closest safe point
            for (int i = 0; i < hits; i++)
            {
                if (Colliders[i] != null && Colliders[i].TryGetComponent<CoverSpline>(out var spline))
                {
                    // 1. Ignore covers too close to the player
                    if (Vector3.Distance(spline.transform.position, TargetPosition) < MinTargetDistance) continue;

                    // --- NEW REJECTION FILTER (DOT PRODUCT) ---
                    // Calculate direction from the cover to the target
                    Vector3 dirToTarget = (TargetPosition - spline.transform.position).normalized;
                    
                    // transform.forward is our outward-facing normal (the red line).
                    float coverAngle = Vector3.Dot(spline.transform.forward, dirToTarget);
                    
                    // If the angle is positive, the player is on the open side of the wall. 
                    // Reject it.
                    if (coverAngle > HideSensitivity) continue;
                    // ------------------------------------------

                    Vector3 closestPt = GetClosestPointOnSpline(spline, dog.transform.position);
                    float dist = Vector3.Distance(dog.transform.position, closestPt);

                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        bestSpline = spline;
                        bestSafePoint = closestPt;
                    }
                }
            }

            if (bestSpline != null)
            {
                currentCover = bestSpline;
                safeHidePosition = bestSafePoint;

                dog.npc.inCover = false;
                dog.npc.SetDestination?.Invoke(safeHidePosition);

                yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance < 0.2f);

                dog.npc.SetAim(false);
                dog.npc.inCover = true;
                
                // Rotate back flat against the wall
                Vector3 wallNormal = currentCover.GetSplineNormalWorld(0);
                dog.transform.rotation = Quaternion.LookRotation(-wallNormal);
            }

            yield return Wait;
        }
    }

    private IEnumerator PeekAtTargetRoutine(Dog dog)
    {
        WaitForSeconds waitBetweenPeeks = new WaitForSeconds(TimeBetweenPeeks);
        WaitForSeconds peekDuration = new WaitForSeconds(PeekDuration);

        while (true)
        {
            // Wait until securely in cover and not already moving
            if (currentCover == null || !dog.npc.inCover || dog.agent.pathPending || Vector3.Distance(dog.transform.position, safeHidePosition) > 0.5f)
            {
                yield return null;
                continue;
            }

            Vector3 TargetPosition = dog.targeting.TargetPosition;

            // 1. Calculate Lateral Step-Out Vector based on Spline Nodes
            Vector3 node0 = currentCover.GetWorldPoint(0);
            Vector3 node1 = currentCover.GetWorldPoint(1);
            Vector3 coverDirection = (node1 - node0).normalized;

            float distToNode0 = Vector3.Distance(node0, TargetPosition);
            float distToNode1 = Vector3.Distance(node1, TargetPosition);

            Vector3 peekPosition;

            // Step out from the node closest to the target
            if (distToNode0 < distToNode1)
            {
                peekPosition = node0 - (coverDirection * PeekOffsetDistance); 
            }
            else
            {
                peekPosition = node1 + (coverDirection * PeekOffsetDistance);
            }

            // 2. Validate position on NavMesh and Execute Step-Out
            if (NavMesh.SamplePosition(peekPosition, out NavMeshHit peekHit, 2f, dog.agent.areaMask))
            {
                dog.npc.SetDestination?.Invoke(peekHit.position);
                yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
                
                isPeeking = true;
                dog.npc.LookAt(TargetPosition); // Face target to shoot

                // Hold peek to shoot
                yield return peekDuration;
                
                isPeeking = false;

                // 3. Retreat back to the safe point on the spline
                dog.npc.SetDestination?.Invoke(safeHidePosition);
                yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
                
                // Snap back to wall
                Vector3 wallNormal = currentCover.GetSplineNormalWorld(0);
                dog.transform.rotation = Quaternion.LookRotation(-wallNormal);
            }
            else
            {
                Debug.LogWarning("NavMesh rejection: Peek position is off the walkable grid.");
            }

            yield return waitBetweenPeeks;
        }
    }

    // Helper math to project AI safely onto the track
    private Vector3 GetClosestPointOnSpline(CoverSpline spline, Vector3 pos)
    {
        Vector3 p1 = spline.GetWorldPoint(0);
        Vector3 p2 = spline.GetWorldPoint(1); 
        
        Vector3 lineDir = p2 - p1;
        float lineLen = lineDir.magnitude;
        lineDir.Normalize();
        
        Vector3 v = pos - p1;
        float d = Vector3.Dot(v, lineDir);
        d = Mathf.Clamp(d, 0f, lineLen); 
        
        return p1 + lineDir * d;
    }
}