using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public Canvas settingsOverlay;
    private bool overlayVisible = false;

    public void toggleSettingsOverlay()
    {
        overlayVisible = !overlayVisible;
        settingsOverlay.gameObject.SetActive(overlayVisible);
    }
}
