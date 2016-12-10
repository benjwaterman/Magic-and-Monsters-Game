using UnityEngine;
using System.Collections;

public class SinkAfterDeath : StateMachineBehaviour
{
    private float timePassed;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponentInChildren<ParticleSystem>().Stop();
        animator.gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.1 && animator.gameObject.GetComponent<Collider>().enabled)
        {
            animator.gameObject.GetComponent<Collider>().enabled = false;
        }

            if (stateInfo.normalizedTime >= 0.9)
        {
            animator.gameObject.transform.position = Vector3.MoveTowards(animator.gameObject.transform.position, animator.gameObject.transform.position - new Vector3(0, 1), Time.deltaTime * 0.3f);
            timePassed += Time.deltaTime;
        }

        if (timePassed >= 3)
            Destroy(animator.gameObject);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
