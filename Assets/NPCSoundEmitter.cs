using UnityEngine;
using System.Collections;

public class NPCSoundEmitter : MonoBehaviour
{
    public float expansionSpeed = 10f;

    public void EmitSound(float intensity)
    {
        GameObject soundSphere = new GameObject("SoundSphere");
        soundSphere.tag = "Sound";
        soundSphere.transform.position = transform.position;
        SphereCollider col = soundSphere.AddComponent<SphereCollider>();
        col.isTrigger = true;
        StartCoroutine(ExpandAndDestroy(col, intensity));
    }

    IEnumerator ExpandAndDestroy(SphereCollider col, float intensity)
    {
        float radius = 0f;
        while (radius < intensity)
        {
            radius += expansionSpeed * Time.deltaTime;
            col.radius = radius;
            yield return null;
        }
        Destroy(col.gameObject);
    }
}
