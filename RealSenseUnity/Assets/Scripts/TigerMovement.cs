using UnityEngine;
using UnityEngine.AI;

public class TigerMovement : MonoBehaviour
{
    public Camera NavAgentDestinationSelectionCamera;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        navMeshAgent = this.gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = new RaycastHit();
            Ray ray = NavAgentDestinationSelectionCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                navMeshAgent.destination = hitInfo.point;
        }

        bool isMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);
    }
}
