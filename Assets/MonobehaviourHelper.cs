using UnityEngine;
using System.Collections;

public class MonoBehaviourHelper : MonoBehaviour
{
    private static MonoBehaviourHelper instance;

    public static MonoBehaviourHelper Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MonoBehaviourHelper");
                instance = go.AddComponent<MonoBehaviourHelper>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    public static Coroutine RunCoroutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine); // Calls MonoBehaviour's StartCoroutine
    }
}