using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateRun : MainCharaStateGround
{
    [SerializeField]
    private float speedThresholdLess = 0.9f;

    protected sealed override void OnStateUpdateGround(Animator animator)
    {
        if (parameters.speed <= speedThresholdLess)
        {
            TransferTo(animator, "Idle");
        }
    }
}
