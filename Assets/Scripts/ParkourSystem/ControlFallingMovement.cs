using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFallingMovement : StateMachineBehaviour
{
    //private void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    //{
    //    animator.GetComponent<PlayerController>().HasPlayerControl = false;
    //}

    //private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    //{
    //    animator.GetComponent<PlayerController>().HasPlayerControl = true;
    //}

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().HasPlayerControl = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().HasPlayerControl = true;
    }
}
