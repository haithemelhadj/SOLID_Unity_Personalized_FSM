using UnityEngine;

[CreateAssetMenu(fileName = "Defend State", menuName = "States List/Player/Defend")]
public class DefendState : LocomotionState
{

    #region  Movement speed
    [Header("Movement Speed")]
    public float deffendMaxSpeed;
    public float deffendAcceleration;
    public float deffendDeceleration;
    #endregion
    public override void CheckSwitchState()
    {
        base.CheckSwitchState();
        if (!IsCurrentState) return;
        if (!currentContext.defendInput)
        {
            SwitchState(factory.GetState(_States.Grounded));
            return;
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        SetMoveSpeed();
        currentContext.defendHitBox.SetActive(true);
        currentContext.canFlip = false;

    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.defendHitBox.SetActive(false);
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

    #region chnage Speed Input

    public override void SetMoveSpeed()
    {
        currentContext.currentMaxMoveSpeed = deffendMaxSpeed;
        currentContext.currentAcceleration = deffendAcceleration;
        currentContext.currentDeceleration = deffendDeceleration;
    }
    #endregion

}
