using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponAmmo : MonoBehaviour
{
    public int clipSize;
    public int extraAmmo;
    [HideInInspector] public int currentAmmo;
    public TextMeshProUGUI primaryAmmo;
    public TextMeshProUGUI secondaryAmmo;

    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip releaseSlideSound;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = clipSize;
    }

    void Update()
    {
        ShowAmmo();
    }

    public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            if (extraAmmo + currentAmmo > clipSize)
            {
                int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                currentAmmo = clipSize;
            }
            else
            {
                currentAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }


    void ShowAmmo()
    {
        if (primaryAmmo) primaryAmmo.text = currentAmmo.ToString();
        if (secondaryAmmo) secondaryAmmo.text = extraAmmo.ToString();
    }
}
