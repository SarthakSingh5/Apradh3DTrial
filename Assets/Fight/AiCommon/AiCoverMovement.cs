#region previous logic
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

#endregion








using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AiCoverMovement : MonoBehaviour
{
    public LayerMask HidableLayers;
    public EnemyLineOfSightChecker LineOfSightChecker;

    [Range(1, 10)]
    public float MinTargetDistance = 5f;

    [Range(-1, 1)]
    [Tooltip("Requires the target to be on the opposite side of the wall. Lower = stricter.")]
    public float HideSensitivity = -0.6f;

    [Range(0, 5f)]
    public float Updatefrequency = 5f;

    [Header("Peeking Settings")]
    public float PeekOffsetDistance = 1f;
    public float TimeBetweenPeeks = 3f;
    public float PeekDuration = 3f;

    private Coroutine peekCoroutine = null;
    private Coroutine movementCoroutine = null;
    private Collider[] Colliders = new Collider[10];

    private Vector3 safeHidePosition;
    public bool isPeeking = false;
    private CoverSpline currentCover;
    public bool failedToFindCover = false;

    private Dog myDog;

    public void StartHiding(Dog dog)
    {
        myDog = dog;
        myDog.agent.updateRotation = false; // PERMANENTLY LOCK NAVMESH ROTATION

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
        // Simply use the current transform. No brain reference needed!
        int hits = Physics.OverlapSphereNonAlloc(transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);
        int validHits = 0;

        for (int i = 0; i < hits; i++)
        {
            if (Colliders[i] != null && Colliders[i].TryGetComponent<CoverSpline>(out var spline))
            {
                if (Vector3.Distance(spline.transform.position, TargetPosition) >= MinTargetDistance)
                {
                    Vector3 dirToTarget = (TargetPosition - spline.transform.position).normalized;
                    // TACTICAL DOT PRODUCT: Uses spline normal, not floor normal
                    float coverAngle = Vector3.Dot(spline.transform.forward, dirToTarget);

                    if (coverAngle <= HideSensitivity)
                    {
                        validHits++;
                    }
                }
            }
        }
        return validHits > 0;
    }

    // --- CONTINUOUS ROTATION DIRECTOR ---
    private void Update()
    {
        if (myDog == null) return;

        if (myDog.npc.inCover)
        {
            // PURE STATE LOGIC: No distance thresholds to cause tweaking!
            if (isPeeking && myDog.targeting.HasTarget)
            {
                Vector3 dirToTarget = myDog.targeting.TargetPosition - myDog.npc.transform.position;
                dirToTarget.y = 0;
                if (dirToTarget.sqrMagnitude > 0.01f)
                {
                    myDog.npc.LookAt?.Invoke(myDog.targeting.TargetPosition);
                }
            }
            else if (currentCover != null)
            {
                Vector3 wallNormal = currentCover.GetSplineNormalWorld(0);
                wallNormal.y = 0f;
                if (wallNormal.sqrMagnitude > 0.01f)
                {
                    // THE FIX: Add a minus sign (-wallNormal) to spin the character 180 degrees!
                    Vector3 lookPos = myDog.npc.transform.position + (-wallNormal * 10f);
                    myDog.npc.LookAt?.Invoke(lookPos);
                }
            }
        }
        else if (myDog.agent.hasPath)
        {
            Vector3 moveDir = myDog.agent.desiredVelocity;
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 0.1f)
            {
                myDog.npc.LookAt?.Invoke(myDog.npc.transform.position + (moveDir * 10f));
            }
        }
    }

    private IEnumerator Hide(Dog dog)
    {
        WaitForSeconds Wait = new WaitForSeconds(Updatefrequency);

        while (true)
        {
            // NULL CHECK: Stop crashing if target dies
            if (isPeeking || !dog.targeting.HasTarget)
            {
                yield return null;
                continue;
            }

            Vector3 TargetPosition = dog.targeting.TargetPosition;
            int hits = Physics.OverlapSphereNonAlloc(dog.npc.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

            CoverSpline bestSpline = null;
            float shortestDist = Mathf.Infinity;
            Vector3 bestSafePoint = Vector3.zero;

            for (int i = 0; i < hits; i++)
            {
                if (Colliders[i] != null && Colliders[i].TryGetComponent<CoverSpline>(out var spline))
                {
                    if (Vector3.Distance(spline.transform.position, TargetPosition) < MinTargetDistance) continue;

                    Vector3 dirToTarget = (TargetPosition - spline.transform.position).normalized;
                    float coverAngle = Vector3.Dot(spline.transform.forward, dirToTarget);

                    if (coverAngle > HideSensitivity) continue;

                    // TIGHT CORNER ANCHOR MATH
                    Vector3 optimalPt = GetOptimalAnchorPoint(spline, TargetPosition);

                    // --- THE NAVMESH REALITY CHECK ---
                    // Force the coordinate onto walkable ground so the agent never gets stranded
                    if (NavMesh.SamplePosition(optimalPt, out NavMeshHit navHit, 2f, dog.agent.areaMask))
                    {
                        optimalPt = navHit.position;
                    }

                    float distToWall = Vector3.Distance(dog.npc.transform.position, optimalPt);

                    if (distToWall < shortestDist)
                    {
                        shortestDist = distToWall;
                        bestSpline = spline;
                        bestSafePoint = optimalPt;
                    }
                }
            }

            if (bestSpline != null)
            {
                bool isNewCover = (currentCover != bestSpline);
                float distToSafePoint = Vector3.Distance(dog.npc.transform.position, bestSafePoint);

                currentCover = bestSpline;
                safeHidePosition = bestSafePoint;
                failedToFindCover = false;

                if (isNewCover || distToSafePoint > 2.0f)
                {
                    dog.npc.SetAim(true);
                    dog.npc.canShoot = true;
                    dog.npc.inCover = false;
                    dog.npc.SetDestination?.Invoke(safeHidePosition);

                    yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                    dog.npc.SetAim(false);
                    dog.npc.canShoot = false;
                    dog.npc.inCover = true;

                    Debug.Log("Hiding");
                }
            }
            else
            {
                failedToFindCover = true;
                dog.npc.inCover = false;
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
            // NULL CHECK
            if (currentCover == null || !dog.npc.inCover || dog.agent.pathPending || failedToFindCover || !dog.targeting.HasTarget)
            {
                yield return null;
                continue;
            }

            yield return waitBetweenPeeks;

            if (currentCover == null || !dog.npc.inCover || failedToFindCover || !dog.targeting.HasTarget) continue;

            Vector3 TargetPosition = dog.targeting.TargetPosition;

            Vector3 node0 = currentCover.GetWorldPoint(0);
            Vector3 node1 = currentCover.GetWorldPoint(1);
            Vector3 coverDirection = (node1 - node0).normalized;

            float distToNode0 = Vector3.Distance(node0, TargetPosition);
            float distToNode1 = Vector3.Distance(node1, TargetPosition);

            Vector3 peekPosition = (distToNode0 < distToNode1)
                ? node0 - (coverDirection * PeekOffsetDistance)
                : node1 + (coverDirection * PeekOffsetDistance);

            // TIGHT 2.0f RADIUS PREVENTS CLIPPING THROUGH WALLS  
            if (NavMesh.SamplePosition(peekPosition, out NavMeshHit peekHit, 2.0f, dog.agent.areaMask))
            {
                isPeeking = true;
                Debug.Log($"Peeking at {peekHit.position}");

                dog.npc.SetDestination?.Invoke(peekHit.position);
                yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                dog.npc.SetAim(true);
                dog.npc.canShoot = true;

                yield return peekDuration;

                if(!dog.targeting.TargetInSight)
                {
                    failedToFindCover = true;
                }

                dog.npc.SetAim(false);
                dog.npc.canShoot = false;

                dog.npc.SetDestination?.Invoke(safeHidePosition);
                yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                isPeeking = false;
            }
            else
            {
                yield return null;
            }
        }
    }

    // --- TIGHT DYNAMIC CORNERS ---
    private Vector3 GetOptimalAnchorPoint(CoverSpline spline, Vector3 targetPos)
    {
        Vector3 node0 = spline.GetWorldPoint(0);
        Vector3 node1 = spline.GetWorldPoint(1);
        Vector3 coverDirection = (node1 - node0).normalized;

        float dist0 = Vector3.Distance(node0, targetPos);
        float dist1 = Vector3.Distance(node1, targetPos);

        float wallLength = Vector3.Distance(node0, node1);
        // Stays tight to the edge (0.2f), preventing the AI from hiding in the center
        float insetBuffer = Mathf.Min(0.2f, wallLength * 0.25f);

        if (dist0 < dist1)
        {
            return node0 + (coverDirection * insetBuffer);
        }
        else
        {
            return node1 - (coverDirection * insetBuffer);
        }
    }

    // --- BULLETPROOF ARRIVAL CHECK ---
    private bool AgentReachedDestination(NavMeshAgent agent)
    {
        if (agent.pathPending) return false;

        // THE FIX: Massive 1.0f buffer so it never gets permanently stuck on a NavMesh barrier
        if (agent.remainingDistance <= agent.stoppingDistance + 1.0f) return true;

        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid) return true;

        return false;
    }
}