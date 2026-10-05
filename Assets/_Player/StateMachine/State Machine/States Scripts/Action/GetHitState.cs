using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GetHit State", menuName = "States List/Player/GetHit")]
public class GetHitState : ActionState
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
        if (currentContext.currentHealth <= 0f && Time.time - enterTime > duration)
        {
            SwitchState(factory.GetState(_States.Death));
            return;
        }
    }

    GameObject hitSource;
    public override void OnEnter()
    {
        base.OnEnter();
        // init
        hitSource = currentContext.HitSource;
        //
        currentContext.lastTimeHit = Time.time;
        TimeStop(stopTimeOnHitDuration);
        currentContext.StartCoroutine(HitFlickering(0.1f));
        ApplyKnockback(hitSource.transform.position);
        Losehealth(getDmgAmount());
    }

    public override void OnExit()
    {
        base.OnExit();
        currentContext.HitSource = null;
        EndKnockBack();
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

    #region On Hit Lose Health 

    public void Losehealth(float amount)
    {
        currentContext.currentHealth -= amount;
    }
    public float getDmgAmount()
    {
        float damageAmount = 0f;

        if (currentContext != null)
        {
            if (hitSource != null)
            {
                DamageSource damageData = hitSource.GetComponent<DamageSource>();
                if (damageData != null)
                {
                    damageAmount = damageData.dmgAmount;
                }
                else
                    Debug.LogError("OnGetHit IDamageSource missing");
            }
            else
                Debug.LogError("OnGetHit HitSource missing");
        }
        else
            Debug.LogError("OnGetHit currentContext missing");

        return damageAmount;
    }


    #endregion

    #region On Hit KnockBack


    public void ApplyKnockback(Vector2 sourcePosition)
    {
        // Calculate direction away from source
        Vector2 direction = ((Vector2)currentContext.transform.position - sourcePosition).normalized;

        // Add vertical component if enabled
        if (currentContext.useVerticalKnockback && Mathf.Abs(direction.y) < 0.3f)
        {
            direction.y = currentContext.verticalKnockbackMultiplier;
            direction.Normalize();
        }

        ApplyKnockbackInDirection(direction);
    }

    public void ApplyKnockbackInDirection(Vector2 direction)
    {
        // Reset velocity
        currentContext.Rb.velocity = Vector2.zero;

        // Set knockback state
        currentContext.isKnockedBack = true;
        currentContext.canMove = false;
        // Initial force application
        currentContext.Rb.AddForce(direction * currentContext.knockbackForce, ForceMode2D.Impulse);
    }

    public void EndKnockBack()
    {
        currentContext.isKnockedBack = false;
        currentContext.canMove = true;
    }

    #endregion

    #region On Hit Invulnerability

    public IEnumerator HitFlickering(float duration)
    {
        if (Time.time - currentContext.lastTimeHit <= currentContext.getHitInvunDuration)
        {
            //set invunrable
            currentContext.isInvunrable = true;
            // start flickering loop
            yield return new WaitForSeconds(duration);
            currentContext.spriteRenderer.color = currentContext.spriteRenderer.color == Color.white ? Color.black : Color.white;
            currentContext.StartCoroutine(HitFlickering(duration));
        }
        else
        {
            // if flickering ends on black make it white
            if (currentContext.spriteRenderer.color != Color.white)
                currentContext.spriteRenderer.color = Color.white;
            // reset invunerability
            currentContext.isInvunrable = false;
        }
    }

    #endregion

    #region Stop Game On Hit
    [Header("Stop Game On Hit")]
    public float stopTimeOnHitDuration = 0.1f;
    public bool gameStopped;

    public void TimeStop(float duration)
    {
        if (gameStopped)
            return;
        Time.timeScale = 0f;
        currentContext.StartCoroutine(ResumeAfterDelay(duration));
    }

    public IEnumerator ResumeAfterDelay(float duration)
    {
        gameStopped = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        gameStopped = false;
    }
    #endregion

    #region 

    #endregion
}