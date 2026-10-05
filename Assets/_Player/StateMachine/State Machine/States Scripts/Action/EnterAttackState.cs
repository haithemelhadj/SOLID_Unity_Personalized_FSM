using UnityEngine;

[CreateAssetMenu(fileName = "Enter Attack State", menuName = "States List/Player/Attacks/Enter Attack")]
public class EnterAttackState : MeleBaseState
{
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (Time.time - enterTime >= duration)
        {
            SwitchState(factory.GetState(_States.GoundAttack));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //attack
        Vector2 newVelocity = new Vector3(0, currentContext.Rb.velocity.y, 0);
        currentContext.Rb.velocity = newVelocity;

    }

    public override void OnExit()
    {
        base.OnExit();
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
