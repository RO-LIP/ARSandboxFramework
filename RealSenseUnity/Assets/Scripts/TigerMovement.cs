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
        if (leftMouseButtonDown() && mouseIsOnScreenOfTargetingCamera())
        {
            RaycastHit hitInfo;
            Ray ray = NavAgentDestinationSelectionCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
                navMeshAgent.destination = hitInfo.point;
        }

        bool tigerIsMoving = navMeshAgent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", tigerIsMoving);
    }

    // only works in build, does not work in editor
    private bool mouseIsOnScreenOfTargetingCamera()
    {
        return Display.RelativeMouseAt(Input.mousePosition).z == NavAgentDestinationSelectionCamera.targetDisplay;
    }

    private bool leftMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }
}
