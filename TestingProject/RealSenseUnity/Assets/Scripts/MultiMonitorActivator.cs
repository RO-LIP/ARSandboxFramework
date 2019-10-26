using UnityEngine;

public class MultiMonitorActivator : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Number of connected displays: " + Display.displays.Length);
        if (Display.displays.Length > 1)
        {
            Debug.Log("Activating Display 2");
            Display.displays[1].Activate();
        }
    }
}