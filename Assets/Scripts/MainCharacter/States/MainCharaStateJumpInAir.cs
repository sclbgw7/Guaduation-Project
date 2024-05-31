using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateJumpInAir : MainCharaStateCanMove
{
    override sealed protected void OnStateUpdateAny(Animator animator)
    {

        // TODO: Maybe you can attack in the air...

        if (parameters.isGround)
        {
            TransferTo(animator, "Jump_Land");
        }
    }

    override protected bool CanRoll()
    {
        return false;
    }
}
