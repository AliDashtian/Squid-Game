using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    private SkinnedMeshRenderer playerMeshRenderer;
    public Material[] skins;

    private void Start()
    {
        playerMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        playerMeshRenderer.material = skins[PlayerPrefs.GetInt("currentSkin", 0)];
    }
}
