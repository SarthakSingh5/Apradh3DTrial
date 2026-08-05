using UnityEngine;

public class CharacterOverlay : MonoBehaviour
{
    public Material mat;

    public Texture2D beard;

    void Start()
    {
        mat.SetTexture("_Beard", beard);
    }
}
