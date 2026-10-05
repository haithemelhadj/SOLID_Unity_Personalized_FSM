using UnityEngine;

[CreateAssetMenu(fileName = "Death State", menuName = "States List/Player/Death")]
public class DeathState : ActionState
{

    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (Time.time - enterTime > duration)
        {
            SwitchState(factory.GetState(_States.Grounded));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        currentContext.Rb.velocity=Vector2.zero;
    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.transform.position = currentContext.respawnPoint.position;
        currentContext.currentHealth = currentContext.maxHealth;

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
