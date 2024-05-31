using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateRoll : MainCharaStateAny
{
    [SerializeField]
    float rollFrame;
    [SerializeField]
    float canDropFrame;
    [SerializeField]
    float moveFrame;
    [SerializeField]
    float invincibleFrame;
    [SerializeField]
    AnimationCurve moveCurve;

    private float canDropTime;
    private float moveBackswing;
    private float invincibleTime;
    private float exitTime;

    private StatedCurve statedCurve;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        rollBackswing = rollFrame / 30.0f;
        canDropTime = canDropFrame / 30.0f;
        moveBackswing = moveFrame / 30.0f;
        invincibleTime = invincibleFrame / 30.0f;

        statedCurve = new StatedCurve(moveCurve);

        parameters.shouldRoll = true;
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        ApplyMoveCurve();

        if (inStateTime >= canDropTime && !parameters.isGround)
        {
            TransferTo(animator, "Jump_InAir");
        }
        if (inStateTime >= moveBackswing)
        {
            CommonMoveBackswingTransfer(animator);
        }
        if (inStateTime >= exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    override protected bool IsInvincible()
    {
        return inStateTime <= invincibleTime;
    }

    private void ApplyMoveCurve()
    {
        parameters.forwardMoveDeltaDis += statedCurve.EvaluateDelta(inStateTime * 30f);
    }
}
