using UnityEngine;

public class EscapeHandler : MonoBehaviour
{
    public GameObject uiPanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if(uiPanel.active)
            {
                uiPanel.SetActive(false);
            } else
            {
                uiPanel.SetActive(true);
            }
    }
}
