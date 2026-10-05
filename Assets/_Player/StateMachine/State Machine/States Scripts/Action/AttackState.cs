using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack State", menuName = "States List/Player/Attacks/Attack")]
public class AttackState : MeleBaseState
{
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (Time.time - enterTime >= duration)
        {
            SwitchState(factory.GetState(_States.Grounded));
            return;
        }
        if (!currentContext.isGrounded && currentContext.Rb.velocity.y < 0.1f)
        {
            SwitchState(factory.GetState(_States.Fall));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        currentContext.attackHitBox.SetActive(true);
        currentContext.canFlip = false;
        currentContext.Rb.velocity = Vector2.zero;
    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.attackHitBox.SetActive(false);
        currentContext.canFlip = true;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
