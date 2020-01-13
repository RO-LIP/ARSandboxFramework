using UnityEngine;

public class MultiMonitorActivator : MonoBehaviour
{
    public bool useSecondMonitor;
    public Camera display2Camera;
    void Start()
    {
        Debug.Log("Number of connected displays: " + Display.displays.Length);
        if (Display.displays.Length > 1 && useSecondMonitor)
        {
            Debug.Log("Activating Display 2");
            display2Camera.gameObject.SetActive(true);
            Display.displays[1].Activate();
        }
    }
}