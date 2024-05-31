using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MainCharaStateCommonAttack : MainCharaStateAny
{
    [SerializeField]
    float rollFrame;
    [SerializeField]
    float nextAttackFrameL;
    [SerializeField]
    float nextAttackFrameR;
    [SerializeField]
    float moveFrame;
    [SerializeField]
    string nextAttackName;
    [SerializeField]
    AnimationCurve moveCurve;
    [SerializeField]
    float moveScaleWhenLocking;
    [SerializeField]
    float applyMoveScaleFrame;
    [System.Serializable]
    private struct AttackEvent
    {
        public BladeAttackParameters parameters;
        public float frameL;
        public float frameR;
    }
    [SerializeField]
    List<AttackEvent> attackEvents;

    private float nextAttackWindowL;
    private float nextAttackWindowR;
    private float moveBackswing;
    private float exitTime;
    private float applyMoveScaleTime;
    private bool reboundHasBegun;

    private StatedCurve statedCurve;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        parameters.fire1Trigger += this.OnTriggerFire1;
        parameters.counterTrigger += this.OnCounterBegin;

        exitTime = stateInfo.length;

        rollBackswing = rollFrame / 30.0f;
        nextAttackWindowL = nextAttackFrameL / 30.0f;
        nextAttackWindowR = nextAttackFrameR / 30.0f;
        moveBackswing = moveFrame / 30.0f;
        applyMoveScaleTime = applyMoveScaleFrame / 30.0f;

        reboundHasBegun = false;

        Debug.Assert(rollBackswing <= exitTime);
        Debug.Assert(nextAttackWindowL <= exitTime);
        Debug.Assert(moveBackswing <= exitTime);

        statedCurve = new StatedCurve(moveCurve);

        // Debug.Log(exitTime);
        // Debug.Log(moveCurve.keys[^1].time);

        parameters.shouldUpdateForward = true;
        // parameters.moveScaleWhenLocking = moveScaleWhenLocking;
        parameters.maxMoveDis = moveCurve.keys[^1].value;
        parameters.isAttacking = true;

        foreach (AttackEvent attackEvent in attackEvents)
        {
            parameters.AddAttackEvent(attackEvent.parameters, attackEvent.frameL / 30.0f, attackEvent.frameR / 30.0f);
        }
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        ApplyMoveCurve();

        // Debug.Log(stateInfo.length);
        if(inStateTime >= nextAttackWindowL)
        {
            if (parameters.shouldFall)
            {
                TransferTo(animator, "Jump_InAir");
            }
        }
        if (inStateTime >= moveBackswing)
        {
            parameters.canMove = true;

            CommonMoveBackswingTransfer(animator);
        }
        if(inStateTime >= exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.fire1Trigger -= this.OnTriggerFire1;
        parameters.counterTrigger -= this.OnCounterBegin;

        parameters.canMove = false;
        parameters.moveScaleWhenLocking = 1.0f;
        parameters.isAttacking = false;
        parameters.bladeIsGleaming = false;

        parameters.SingleAttackOut();
    }

    private bool OnTriggerFire1(Animator animator)
    {
        if (inStateTime >= nextAttackWindowL)
        { 
            if(inStateTime <= nextAttackWindowR)
            {
                if (nextAttackName != null && nextAttackName != "")
                {
                    TransferTo(animator, nextAttackName, 0.15f);
                    return true;
                }
            }
            else
            {
                TransferTo(animator, "Attack01");
            }
        }
        return false;
    }

    private void OnCounterBegin(Animator animator)
    {
        reboundHasBegun = true;
        TransferTo(animator, "Rebound", 0.01f);
    }
    
    private void ApplyMoveCurve()
    {
        // TODO: Sometimes the OnStateUpdate will be called more than once per frame.
        // I'm confused about it.
        // So there can't be "=", must be "+=".

        if (inStateTime <= applyMoveScaleTime)
        {
            parameters.moveScaleWhenLocking = moveScaleWhenLocking;
        }
        else
        {
            parameters.moveScaleWhenLocking = 1.0f;
        }

        parameters.forwardMoveDeltaDis += statedCurve.EvaluateDelta(inStateTime * 30f);
    }

    protected override bool IsInvincible()
    {
        return reboundHasBegun;
    }
}
