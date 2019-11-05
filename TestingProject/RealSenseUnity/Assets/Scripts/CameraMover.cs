using UnityEngine;

public class CameraMover : MonoBehaviour
{
    void Update()
    {
        var camera = this.GetComponent<Camera>();

        if (Input.GetKey(KeyCode.A))
            this.transform.position = this.transform.position + Vector3.right;
        if (Input.GetKey(KeyCode.D))
            this.transform.position = this.transform.position + Vector3.left;
        if (Input.GetKey(KeyCode.W))
            this.transform.position = this.transform.position + Vector3.back;
        if (Input.GetKey(KeyCode.S))
            this.transform.position = this.transform.position + Vector3.forward;
        if (Input.GetKey(KeyCode.Q))
            camera.orthographicSize = camera.orthographicSize - 1f;
        if (Input.GetKey(KeyCode.E))
            camera.orthographicSize = camera.orthographicSize + 1f;
    }
}
