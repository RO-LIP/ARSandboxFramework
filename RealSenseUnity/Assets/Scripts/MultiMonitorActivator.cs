using UnityEngine;

public class MultiMonitorActivator : MonoBehaviour
{
    public bool useSecondMonitor;
    public Camera display2Camera;
    void Start()
    {
        if (Display.displays.Length > 1 && useSecondMonitor)
        {
            display2Camera.gameObject.SetActive(true);
            Display.displays[1].Activate();
        }
    }
}