using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MainCharaStateJumpBegin : MainCharaStateCanMove
{
    private float exitTime;

    override sealed protected void OnStateEnterCanMove(Animator animator)
    {
        exitTime = stateInfo.length;
        parameters.isJumping = true;
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        if(inStateTime >= exitTime)
        {
            TransferTo(animator, "Jump_InAir");
        }
    }

    override protected bool CanRoll()
    {
        return false;
    }
}
