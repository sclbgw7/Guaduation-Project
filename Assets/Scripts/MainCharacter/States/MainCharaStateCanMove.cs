using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainCharaStateCanMove : MainCharaStateAny
{
    override sealed protected void OnStateEnterAny(Animator animator)
    {
        parameters.canMove = true;

        OnStateEnterCanMove(animator);
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.canMove = false;

        OnStateOutCanMove(animator);
    }

    // interfaces  ----------------------------------------

    virtual protected void OnStateEnterCanMove(Animator animator)
    {
        ;
    }

    virtual protected void OnStateOutCanMove(Animator animator)
    {
        ;
    }
}
