using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateIdle : MainCharaStateGround
{
    [SerializeField]
    private float speedThresholdGreater = 0.1f;

    protected sealed override void OnStateUpdateGround(Animator animator)
    {
        if (parameters.speed >= speedThresholdGreater)
        {
            TransferTo(animator, "Run");
        }
    }
}
