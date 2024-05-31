using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateRise : MainCharaStateAny
{
    [SerializeField]
    float rollFrame;
    [SerializeField]
    float moveFrame;

    private float exitTime;
    private float moveBackswing;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        rollBackswing = rollFrame / 30.0f;
        moveBackswing = moveFrame / 30.0f;
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        if (inStateTime >= moveBackswing)
        {
            parameters.canMove = true;

            CommonMoveBackswingTransfer(animator);
        }
        if (inStateTime >= exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.canMove = false;
    }
}
