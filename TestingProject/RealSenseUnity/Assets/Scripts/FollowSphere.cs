using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSphere : MonoBehaviour
{
    public Camera cam;
    private Vector3 camOffset = new Vector3(0,20,-25);

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = transform.position + camOffset;
    }
}
