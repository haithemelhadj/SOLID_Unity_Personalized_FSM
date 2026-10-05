using UnityEngine;

[CreateAssetMenu(fileName = "Exit Attack State", menuName = "States List/Player/Attacks/Exit Attack")]
public class ExitAttackState : MeleBaseState
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
    }

    public override void OnEnter()
    {
        base.OnEnter();
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
