using UnityEngine;

public class CloseApplication : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Exiting Game...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
