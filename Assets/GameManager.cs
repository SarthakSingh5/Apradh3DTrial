using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        Camera.main.gameObject.AddComponent<CinemachineBrain>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
