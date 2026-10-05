using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash State", menuName = "States List/Player/Dash")]
public class DashState : ActionState
{
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (currentContext.isHuggingWall)
        {
            SwitchState(factory.GetState(_States.WallSlide));
            return;
        }
        else if (!currentContext.isDashing)
        {
            SwitchState(factory.GetState(_States.Fall));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        currentContext.isDashing = true;
        currentContext.dashReset = false;
        if(currentContext.isInvunDuringDash) currentContext.isInvunrable = true;
        currentContext.StartCoroutine(Dash());
    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.lastDashFinishTime = Time.time;
        if (currentContext.isInvunDuringDash) currentContext.isInvunrable = false;
    }


    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnLateUpdate()
    {
        if (!currentContext.isDashing)
            base.OnLateUpdate();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    #region Dash
    public IEnumerator Dash()
    {
        //set dash vars
        currentContext.isDashing = true;

        //save gravity
        float originalGravity = currentContext.Rb.gravityScale;
        currentContext.Rb.gravityScale = 0f;


        //null velocity
        currentContext.Rb.velocity = Vector2.zero;

        //set dash direction if is wall sliding
        if (currentContext.isHuggingWall)
            currentContext.transform.localScale = new Vector2(-currentContext.transform.localScale.x, currentContext.transform.localScale.y);

        // Apply dash velocity
        currentContext.Rb.velocity = new Vector2(Mathf.Sign(currentContext.transform.localScale.x) * currentContext.dashForce, 0f);

        // Wait for dash duration (FPS-independent)
        yield return new WaitForSeconds(currentContext.dashDuration);

        // Reset physics and gravity
        currentContext.Rb.gravityScale = originalGravity;
        currentContext.Rb.velocity = Vector2.zero;

        currentContext.isDashing = false;
    }

    #endregion
}
