#region previous logic
// using System.Collections;
// using UnityEngine;
// using UnityEngine.AI;
// using System.Collections.Generic;

// public class AiCoverMovement : MonoBehaviour
// {
//     public LayerMask HidableLayers;
//     public EnemyLineOfSightChecker LineOfSightChecker;

//     [Range(-1, 1)]
//     [Tooltip("Lower is a better hiding spot")]
//     public float HideSensitivity = 0;

//     [Range(1, 10)]
//     public float MinTargetDistance = 5f;

//     [Range(0, 5f)]
//     public float MinObstacleHeight = 1.25f;

//     [Range(0, 5f)]
//     public float Updatefrequency = 5f; // seconds
//     [Range(5f, 50f)]
//     public float MaxTargetDistance = 15f; // if target moves beyond this, stop hiding

//     private Coroutine peekCoroutine = null;
//     private Coroutine movementCoroutine = null;
//     private Collider[] Colliders = new Collider[10]; // more is less performant, but more options

//     [Header("Peeking Settings")]
//     public float PeekOffsetDistance = 1f;
//     public float TimeBetweenPeeks = 3f;
//     public float PeekDuration = 1f;

//     private Vector3 lastHidePosition;
//     public bool isPeeking = false;


//     public void StartHiding(Dog dog)
//     {
//         StopHiding(dog);
//         movementCoroutine = StartCoroutine(Hide(dog));
//         peekCoroutine = StartCoroutine(PeekAtTargetRoutine(dog));

//     }


//     public void StopHiding(Dog dog)
//     {
//         if (movementCoroutine != null)
//         {
//             StopCoroutine(movementCoroutine);
//             movementCoroutine = null;
//         }
//         if (peekCoroutine != null)
//         {
//             StopCoroutine(peekCoroutine);
//             peekCoroutine = null;
//         }
//         isPeeking = false;
//         dog.npc.inCover = false;

//     }

//     public bool HasAnyCover(Vector3 TargetPosition)
//     {
//         int hits = Physics.OverlapSphereNonAlloc(transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

//         int validHits = 0;

//         for (int i = 0; i < hits; i++)
//         {
//             if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) >= MinTargetDistance &&
//                 Colliders[i].bounds.size.y >= MinObstacleHeight)
//             {
//                 validHits++;
//             }
//         }

//         return validHits > 0;
//     }


//     private IEnumerator Hide(Dog dog)
//     {
//         if (peekCoroutine != null)
//         {
//             StopCoroutine(peekCoroutine);
//             peekCoroutine = null;
//         }
//         WaitForSeconds Wait = new WaitForSeconds(Updatefrequency);


//         while (true)
//         {
//             Vector3 TargetPosition = dog.targeting.TargetPosition;
//             for (int i = 0; i < Colliders.Length; i++)
//             {
//                 Colliders[i] = null;
//             }

//             int hits = Physics.OverlapSphereNonAlloc(dog.transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);

//             int hitReduction = 0;
//             for (int i = 0; i < hits; i++)
//             {
//                 if (Vector3.Distance(Colliders[i].transform.position, TargetPosition) < MinTargetDistance || Colliders[i].bounds.size.y < MinObstacleHeight)
//                 {
//                     Colliders[i] = null;
//                     hitReduction++;
//                 }
//             }

//             hits -= hitReduction;

//             System.Array.Sort(Colliders, ColliderArraySortComparer);

//             for (int i = 0; i < hits; i++)
//             {
//                 if (Colliders[i].bounds.size.y >= 3)
//                 {
//                     List<Vector3> candidatePoints = GenerateCoverPoints(Colliders[i]);

//                     foreach (var point in candidatePoints)
//                     {
//                         if (NavMesh.SamplePosition(point, out NavMeshHit hit, 5f, dog.agent.areaMask))
//                         {
//                             if (!NavMesh.FindClosestEdge(hit.position, out hit, dog.agent.areaMask))
//                             {
//                                 Debug.LogError($"Unable to find edge close to (hit.position)");
//                             }
//                             // Vector3 rotated = Quaternion.Euler(0, -90, 0) * hit.normal;
//                             // Vector3 a = hit.normal.normalized;
//                             // Vector3 b = (TargetPosition - hit.position).normalized;

//                             // float sinTheta = Vector3.Cross(a, b).magnitude;
//                             // float cosTheta = Vector3.Dot(a, b);
//                             // float tanTheta = sinTheta / cosTheta;
//                             if (Vector3.Dot(hit.normal, (TargetPosition - hit.position).normalized) < HideSensitivity)
//                             {

//                                 dog.npc.inCover = false;
//                                 lastHidePosition = hit.position;



//                                 dog.npc.SetDestination?.Invoke(lastHidePosition);

//                                 yield return new WaitUntil(() =>!dog.agent.pathPending && dog.agent.remainingDistance < 0.2f);

//                                 dog.npc.SetAim(false);
//                                 dog.npc.inCover = true;
//                                 Vector3 pos = dog.transform.position;
//                                 pos += hit.normal * 1000.0f;
//                                 pos += Vector3.up * 1.5f;
//                                 dog.npc.LookAt(pos);

//                                 break;

//                             }
//                         }
//                         else
//                         {
//                             Debug.LogError($"Unable to find NavMesh near object (Colliders[{i}].name) at {Colliders[i].transform.position}");
//                         }
//                     }
//                 }
//                 else
//                 {
//                     if (NavMesh.SamplePosition(Colliders[i].transform.position, out NavMeshHit hit1, 20f, dog.agent.areaMask))
//                     {
//                         if (!NavMesh.FindClosestEdge(hit1.position, out hit1, dog.agent.areaMask))
//                         {
//                             Debug.LogError($"Unable to find edge close to {hit1.position}");
//                         }

//                         if (Vector3.Dot(hit1.normal, (TargetPosition - hit1.position).normalized) < HideSensitivity)
//                         {
//                             dog.npc.SetDestination?.Invoke(hit1.position);
//                             break;
//                         }
//                         else
//                         {
//                             if (NavMesh.SamplePosition(Colliders[i].transform.position - (TargetPosition - hit1.position).normalized * 2, out NavMeshHit hit2, 20f, dog.agent.areaMask))
//                             {
//                                 if (!NavMesh.FindClosestEdge(hit2.position, out hit2, dog.agent.areaMask))
//                                 {
//                                     Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
//                                 }

//                                 if (Vector3.Dot(hit2.normal, (TargetPosition - hit2.position).normalized) < HideSensitivity)
//                                 {
//                                     dog.npc.SetDestination?.Invoke(hit2.position);
//                                     break;
//                                 }
//                             }
//                         }
//                     }
//                     else
//                     {
//                         Debug.LogError($"Unable to find NavMesh near object {Colliders[i].name} at {Colliders[i].transform.position}");
//                     }

//                 }

//             }
//             yield return Wait;

//         }
//     }

//     private int ColliderArraySortComparer(Collider A, Collider B)
//     {
//         if (A == null && B != null)
//         {
//             return 1;
//         }
//         else if (A != null && B == null)
//         {
//             return -1;
//         }
//         else if (A == null && B == null)
//         {
//             return 0;
//         }
//         else
//         {
//             return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
//         }
//     }

//     List<Vector3> GenerateCoverPoints(Collider collider)
//     {
//         List<Vector3> points = new List<Vector3>();
//         Bounds bounds = collider.bounds;
//         Vector3 center = bounds.center;
//         Vector3 extents = bounds.extents;

//         float inset = 0.2f; // distance to move inward from the actual corner

//         // X and Z directions
//         float x = extents.x;
//         float z = extents.z;

//         // 8 face-near-corner points (slightly inward from corners)
//         points.Add(center + new Vector3(-x + inset, 0, z)); // front-left (inward on x)
//         points.Add(center + new Vector3(-x, 0, z - inset)); // front-left (inward on z)

//         points.Add(center + new Vector3(x - inset, 0, z)); // front-right
//         points.Add(center + new Vector3(x, 0, z - inset)); // front-right

//         points.Add(center + new Vector3(-x + inset, 0, -z)); // back-left
//         points.Add(center + new Vector3(-x, 0, -z + inset)); // back-left

//         points.Add(center + new Vector3(x - inset, 0, -z)); // back-right
//         points.Add(center + new Vector3(x, 0, -z + inset)); // back-right

//         return points;
//     }


//     private IEnumerator PeekAtTargetRoutine(Dog dog)
//     {
//         WaitForSeconds waitBetweenPeeks = new WaitForSeconds(TimeBetweenPeeks);
//         WaitForSeconds peekDuration = new WaitForSeconds(PeekDuration);

//         while (true)
//         {
//             Vector3 TargetPosition = dog.targeting.TargetPosition;

//             // Wait until agent is in cover and not moving
//             if (dog.agent.pathPending || Vector3.Distance(transform.position, lastHidePosition) > 0.5f)
//             {
//                 yield return null;
//                 continue;
//             }

//             // Ensure no peeking if state is about to change
//             if (!dog.npc.inCover)
//             {
//                 yield return null;
//                 continue;
//             }


//             Vector3 directionToTarget = (TargetPosition - lastHidePosition).normalized;
//             Vector3 peekPosition = lastHidePosition + directionToTarget * PeekOffsetDistance;

//             if (NavMesh.SamplePosition(peekPosition, out NavMeshHit peekHit, 2f, dog.agent.areaMask))
//             {
//                 dog.npc.SetDestination?.Invoke(peekHit.position);
//                 yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
//                 isPeeking = true;

//                 // Ensure agent stays at peek position for the full duration
//                 yield return peekDuration;
//                 isPeeking = false;

//                 // Return to cover position
//                 dog.npc.SetDestination?.Invoke(lastHidePosition);
//                 yield return new WaitUntil(() => !dog.agent.pathPending && dog.agent.remainingDistance <= 0.2f);
//             }
//             else
//             {
//                 Debug.LogWarning($"No valid NavMesh position for peek at {peekPosition}");
//             }

//             // Wait before next peek
//             yield return waitBetweenPeeks;
//         }
//     }

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

    [Header("Tactical Memory")]
    public float BlacklistResetDistance = 4.0f; // Target must move this far to un-blacklist a cover

    private Coroutine peekCoroutine = null;
    private Coroutine movementCoroutine = null;
    private Collider[] Colliders = new Collider[200];

    private Vector3 safeHidePosition;
    public bool isPeeking = false;
    private CoverSpline currentCover;
    public bool failedToFindCover = false;

    // THE BLACKLIST: Stores a failed cover and the exact TargetPosition when it failed
    private Dictionary<CoverSpline, Vector3> blacklistedCovers = new Dictionary<CoverSpline, Vector3>();

    [Tooltip("Layers that block bullets/vision (e.g., Default, Environment, Obstacles)")]
    public LayerMask VisionBlockingLayers;

    [Header("Tactical Movement")]
    [Tooltip("How many times the AI will shoot before moving to flank")]
    public int MaxPeeksBeforeMoving = 3;

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
        // blacklistedCovers.Clear(); // Clear memory when exiting combat state
    }

    public bool HasAnyCover(Vector3 TargetPosition)
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, LineOfSightChecker.Collider.radius, Colliders, HidableLayers);
        int validHits = 0;

        for (int i = 0; i < hits; i++)
        {
            if (Colliders[i] != null && Colliders[i].TryGetComponent<CoverSpline>(out var spline))
            {
                // --- TACTICAL MEMORY SYNC ---
                // Before saying this cover is valid, check if it's on the blacklist!
                if (blacklistedCovers.TryGetValue(spline, out Vector3 failedPlayerPos))
                {
                    if (Vector3.Distance(TargetPosition, failedPlayerPos) < BlacklistResetDistance)
                    {
                        // The player hasn't moved enough. This cover is still blacklisted.
                        continue; 
                    }
                }

                // --- STANDARD COVER MATH ---
                if (Vector3.Distance(spline.transform.position, TargetPosition) >= MinTargetDistance)
                {
                    Vector3 dirToTarget = (TargetPosition - spline.transform.position).normalized;
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

    private void Update()
    {
        if (myDog == null) return;

        if (myDog.npc.inCover)
        {
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
                    // --- 1. TACTICAL MEMORY CHECK ---
                    if (blacklistedCovers.TryGetValue(spline, out Vector3 failedPlayerPos))
                    {
                        if (Vector3.Distance(TargetPosition, failedPlayerPos) < BlacklistResetDistance)
                        {
                            // Player is still in the same spot. This cover is known to be bad. Skip it!
                            continue;
                        }
                        else
                        {
                            // Player moved significantly! We can try this cover again.
                            blacklistedCovers.Remove(spline);
                        }
                    }

                    // --- 2. STANDARD COVER MATH ---
                    if (Vector3.Distance(spline.transform.position, TargetPosition) < MinTargetDistance) continue;

                    Vector3 dirToTarget = (TargetPosition - spline.transform.position).normalized;
                    float coverAngle = Vector3.Dot(spline.transform.forward, dirToTarget);

                    if (coverAngle > HideSensitivity) continue;

                    Vector3 optimalPt = GetOptimalAnchorPoint(spline, TargetPosition);

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
                    dog.npc.inCover = false;
                    dog.npc.SetDestination?.Invoke(safeHidePosition);

                    yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                    dog.npc.inCover = true;
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
        int currentPeekCount = 0; // NEW: Track how many shots we've taken

        while (true)
        {
            if (currentCover == null || !dog.npc.inCover || dog.agent.pathPending || failedToFindCover || !dog.targeting.HasTarget)
            {
                yield return null;
                continue;
            }

            yield return waitBetweenPeeks;

            if (currentCover == null || !dog.npc.inCover || failedToFindCover || !dog.targeting.HasTarget) continue;

            Vector3 TargetPosition = dog.targeting.TargetPosition;
            Vector3 targetEyeLevel = TargetPosition + Vector3.up * 1.5f;

            Vector3 node0 = currentCover.GetWorldPoint(0);
            Vector3 node1 = currentCover.GetWorldPoint(1);
            
            // Vector running ALONG the wall (sideways)
            Vector3 coverDirection = (node1 - node0).normalized;

            // Get normals for both ends to know which way is "outward" into the room
            Vector3 normal0 = currentCover.GetSplineNormalWorld(0);
            normal0.y = 0f;
            normal0.Normalize();

            Vector3 normal1 = currentCover.GetSplineNormalWorld(1);
            normal1.y = 0f;
            normal1.Normalize();

            // --- THE BEVEL FIX: HYBRID STEP ---
            // Step sideways (PeekOffsetDistance) AND slightly outward (0.4f) to clear thick/angled doorframes
            float outwardStep = 0.4f;
            Vector3 peekPos0 = node0 - (coverDirection * PeekOffsetDistance) + (normal0 * outwardStep);
            Vector3 peekPos1 = node1 + (coverDirection * PeekOffsetDistance) + (normal1 * outwardStep);

            // Calculate eye levels for raycasting
            Vector3 eyeLevel0 = peekPos0 + Vector3.up * 1.5f;
            Vector3 eyeLevel1 = peekPos1 + Vector3.up * 1.5f;
            
            // Calculate eye levels at the exact edge of the cover (starting point for the step check)
            Vector3 nodeEye0 = node0 + Vector3.up * 1.5f;
            Vector3 nodeEye1 = node1 + Vector3.up * 1.5f;

            // Used for the final standing spot and shooting line (shoulder width)
            float checkRadius = 0.25f; 

            // 1. THE STEP CHECK (Diagonal Raycast)
            // Checks the true diagonal path from the node to the offset peek spot
            bool step0IsClear = !Physics.Raycast(nodeEye0, (eyeLevel0 - nodeEye0).normalized, Vector3.Distance(nodeEye0, eyeLevel0), VisionBlockingLayers);
            bool step1IsClear = !Physics.Raycast(nodeEye1, (eyeLevel1 - nodeEye1).normalized, Vector3.Distance(nodeEye1, eyeLevel1), VisionBlockingLayers);

            // 2. THE DESTINATION CHECK (CheckSphere)
            // Ensures the final peek position we stepped to isn't shoved inside a solid pillar
            bool pos0IsPhysicallyClear = step0IsClear && !Physics.CheckSphere(eyeLevel0, checkRadius, VisionBlockingLayers);
            bool pos1IsPhysicallyClear = step1IsClear && !Physics.CheckSphere(eyeLevel1, checkRadius, VisionBlockingLayers);

            // 3. THE LINE OF SIGHT CHECK (Thick SphereCast)
            // Uses the thick radius to ensure the NPC's shoulders and gun won't clip through the corner when shooting
            bool canSeeFrom0 = pos0IsPhysicallyClear && !Physics.SphereCast(eyeLevel0, checkRadius, (targetEyeLevel - eyeLevel0).normalized, out _, Vector3.Distance(eyeLevel0, targetEyeLevel), VisionBlockingLayers);
            bool canSeeFrom1 = pos1IsPhysicallyClear && !Physics.SphereCast(eyeLevel1, checkRadius, (targetEyeLevel - eyeLevel1).normalized, out _, Vector3.Distance(eyeLevel1, targetEyeLevel), VisionBlockingLayers);

            Vector3 finalPeekPosition = Vector3.zero;

            // --- SMART NODE SELECTION LOGIC --- 
            if (canSeeFrom0 && canSeeFrom1)
            {
                // Both are clear! Pick the one physically closest to the player
                float dist0 = Vector3.Distance(node0, TargetPosition);
                float dist1 = Vector3.Distance(node1, TargetPosition);
                finalPeekPosition = (dist0 < dist1) ? peekPos0 : peekPos1;
            }
            else if (canSeeFrom0)
            {
                finalPeekPosition = peekPos0; // Only left is clear
            }
            else if (canSeeFrom1)
            {
                finalPeekPosition = peekPos1; // Only right is clear
            }
            else
            {
                // BOTH SIDES ARE BLOCKED BY PROPS/WALLS!
                // Add to blacklist so the AI doesn't keep trying this dead-end wall
                Debug.Log("Both peek positions blocked.");
                blacklistedCovers[currentCover] = TargetPosition;
                failedToFindCover = true;
                
                // Immediately return to cover loop to find a new wall
                yield return null;
                continue;
            }

            // Execute the valid peek
            if (NavMesh.SamplePosition(finalPeekPosition, out NavMeshHit peekHit, 2.0f, dog.agent.areaMask))
            {
                isPeeking = true;
                dog.npc.SetDestination?.Invoke(peekHit.position);
                yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                
                Debug.DrawLine(dog.transform.position + Vector3.up * 1.5f, dog.targeting.TargetPosition, Color.red, 2.0f);

                yield return peekDuration;

                if(!dog.targeting.TargetInSight)
                {
                    Debug.Log("Peek failed");
                    failedToFindCover = true;
                }


                dog.npc.SetDestination?.Invoke(safeHidePosition);
                yield return new WaitUntil(() => AgentReachedDestination(dog.agent));

                isPeeking = false;

                // --- NEW: TACTICAL REPOSITIONING / FLANKING ---
                currentPeekCount++;
                if (currentPeekCount >= MaxPeeksBeforeMoving)
                {
                    // Add this perfectly good cover to the blacklist temporarily 
                    // so the AI is forced to find a DIFFERENT spot.
                    blacklistedCovers[currentCover] = TargetPosition; 
                    
                    // Force the state machine to drop into Engage and walk around
                    failedToFindCover = true; 
                    
                    // Reset the counter for the next cover it finds
                    currentPeekCount = 0; 
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private Vector3 GetOptimalAnchorPoint(CoverSpline spline, Vector3 targetPos)
    {
        Vector3 node0 = spline.GetWorldPoint(0);
        Vector3 node1 = spline.GetWorldPoint(1);
        Vector3 coverDirection = (node1 - node0).normalized;

        float dist0 = Vector3.Distance(node0, targetPos);
        float dist1 = Vector3.Distance(node1, targetPos);

        float wallLength = Vector3.Distance(node0, node1);
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

    private bool AgentReachedDestination(NavMeshAgent agent)
    {
        if (agent.pathPending) return false;
        if (agent.remainingDistance <= agent.stoppingDistance + 1.0f) return true;
        if (!agent.hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid) return true;

        return false;
    }
}