using System.Collections.Generic;
using UnityEngine;

public class ModularClothFitter : MonoBehaviour
{
    [Tooltip("Drag the main animated character's body mesh here")]
    public SkinnedMeshRenderer targetCharacterSMR;

    [Tooltip("Drag the Vest's own Game_engine object here so it can be deleted at runtime")]
    public GameObject oldSkeletonRoot; 

    void Start()
    {
        SkinnedMeshRenderer clothSMR = GetComponent<SkinnedMeshRenderer>();
        if (clothSMR == null || targetCharacterSMR == null) return;

        // 1. Build a dictionary of the main character's bones for fast lookup
        Dictionary<string, Transform> targetBoneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in targetCharacterSMR.bones)
        {
            if (!targetBoneMap.ContainsKey(bone.name))
            {
                targetBoneMap.Add(bone.name, bone);
            }
        }

        // 2. Create a new bone array for the clothing
        Transform[] newClothBones = new Transform[clothSMR.bones.Length];
        
        for (int i = 0; i < clothSMR.bones.Length; i++)
        {
            // Failsafe in case a bone is already missing
            if (clothSMR.bones[i] == null) continue; 

            string boneName = clothSMR.bones[i].name;

            if (targetBoneMap.TryGetValue(boneName, out Transform targetBone))
            {
                newClothBones[i] = targetBone; // Swap to the animated bone
            }
            else
            {
                newClothBones[i] = clothSMR.bones[i]; 
            }
        }

        // 3. Reassign the bones array and root bone
        clothSMR.bones = newClothBones;

        if (clothSMR.rootBone != null && targetBoneMap.TryGetValue(clothSMR.rootBone.name, out Transform targetRoot))
        {
            clothSMR.rootBone = targetRoot;
        }

        // 4. CLEANUP: Destroy the vest's original, static skeleton to save memory
        if (oldSkeletonRoot != null)
        {
            Destroy(oldSkeletonRoot);
        }
    }
}