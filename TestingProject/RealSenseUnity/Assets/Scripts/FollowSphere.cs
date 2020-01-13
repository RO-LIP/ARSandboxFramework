using UnityEngine;

public class FollowSphere : MonoBehaviour
{
    public Camera cam;
    private Vector3 camOffset = new Vector3(0,20,-25);
    void Update()
    {
        cam.transform.position = transform.position + camOffset;
    }
}
