
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateLand : MainCharaStateGround
{
    [SerializeField]
    private float speedThresholdGreater = 0.1f;
    
    private float exitTime;

    override sealed protected void OnStateEnterGround(Animator animator)
    {
        exitTime = stateInfo.length;
        parameters.isJumping = false;
    }

    protected sealed override void OnStateUpdateGround(Animator animator)
    {
        if (inStateTime >= exitTime / 3.0f)
        {
            if (parameters.speed >= speedThresholdGreater)
            {
                TransferTo(animator, "Run");
            }
        }
        if (inStateTime >= exitTime)
        {
            TransferTo(animator, "Idle");
        }
    }
}
