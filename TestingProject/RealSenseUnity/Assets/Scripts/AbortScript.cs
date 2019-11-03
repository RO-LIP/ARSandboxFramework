using UnityEngine;

public class AbortScript : MonoBehaviour
{
    public GameObject uiPanel;

    public void Abort()
    {
        if (uiPanel.active)
        {
            uiPanel.SetActive(false);
        }
        else
        {
            uiPanel.SetActive(true);
        }
    }
}
