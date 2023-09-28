using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    //public float speed = GameManager.Instance.BossSpeed;
    public float Range = 3f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<BossController>().Flip();
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 trgtPos =  Vector2.MoveTowards(rb.position, target, GameManager.Instance.BossSpeed * Time.fixedDeltaTime);
        rb.MovePosition(trgtPos);
        if (Vector2.Distance(player.position, rb.position) <= Range)
        {
            animator.SetTrigger("SetAttack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("SetAttack");
    }
}
