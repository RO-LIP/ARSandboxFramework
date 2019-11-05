using UnityEngine;

public class ResetSpherePosition : MonoBehaviour
{
    private Vector3 startPosition;
    private Rigidbody sphereRigidBody;
    void Start()
    {
        startPosition = transform.position;
        sphereRigidBody = GetComponent<Rigidbody>();
    }

    public void resetSpherePosition()
    {
        transform.position = startPosition;
        sphereRigidBody.velocity = new Vector3(0, 0, 0);
        sphereRigidBody.angularVelocity = new Vector3(0, 0, 0);
    }
}
