using UnityEngine;

[CreateAssetMenu(fileName = "Jump State", menuName = "States List/Player/Jump")]
public class JumpState : LocomotionState
{
    #region  Movement Speed
    [Header("Movement Speed")]
    public float jumpMaxSpeed;
    public float jumpAcceleration;
    public float jumpDeceleration;
    #endregion
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (currentContext.jumpInputUp || currentContext.jumpTimeCounter < 0 || currentContext.isHeadBumping)
        {
            SwitchState(factory.GetState(_States.Fall));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        SetMoveSpeed();
        //set jump start counter
        currentContext.willBufferJump = false;
        currentContext.canCyoteJump = false;
        currentContext.jumpTimeCounter = currentContext.maxJumpTime;
        //animator
        
        //null y velocity
        currentContext.Rb.velocity = new Vector2(currentContext.Rb.velocity.x, 0f);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        currentContext.jumpTimeCounter -= Time.deltaTime;

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        // Apply initial jump impulse
        Jump();
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
    }
    public override void OnExit()
    {
        base.OnExit();
        currentContext.willBufferJump = false;


        currentContext.Rb.velocity = new Vector2(currentContext.Rb.velocity.x, currentContext.Rb.velocity.y * 0.3f);
    }

    private void Jump()
    {
        //wall jumping + can't move 
        if (Time.time - currentContext.wallJumpPressTime < currentContext.wallJumpDuration)
        {
            currentContext.jumpDirection = new Vector2(-currentContext.transform.localScale.x * currentContext.wallJumpDirection.x, currentContext.wallJumpDirection.y);

            //Debug.Log("wall juming");
        }
        else // else if is normal jumping
        {
            currentContext.jumpDirection = new Vector2(currentContext.Rb.velocity.x, currentContext.jumpForce);
        }

        currentContext.Rb.velocity = currentContext.jumpDirection;
    }

    #region change Speed Input

    public override void SetMoveSpeed()
    {

        currentContext.currentMaxMoveSpeed = jumpMaxSpeed;
        currentContext.currentAcceleration = jumpAcceleration;
        currentContext.currentDeceleration = jumpDeceleration;
    }
    #endregion
}
