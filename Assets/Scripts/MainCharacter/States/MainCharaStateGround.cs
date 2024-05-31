using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainCharaStateGround : MainCharaStateCanMove
{
    override sealed protected void OnStateUpdateAny(Animator animator)
    {

        if(parameters.shouldFall)
        {
            TransferTo(animator, "Jump_InAir");
        }

        OnStateUpdateGround(animator);
    }

    override sealed protected void OnStateEnterCanMove(Animator animator)
    {
        // parameters.jumpTrigger += this.OnTriggerJump;
        parameters.fire1Trigger += this.OnTriggerFire1;

        OnStateEnterGround(animator);
    }

    override sealed protected void OnStateOutCanMove(Animator animator)
    {
        // parameters.jumpTrigger -= this.OnTriggerJump;
        parameters.fire1Trigger -= this.OnTriggerFire1;

        OnStateOutGround(animator);
    }

    

    private bool OnTriggerFire1(Animator animator)
    {
        // Debug.Log("Attacking01!");
        TransferTo(animator, "Attack01");
        return true;
    }

    virtual protected void OnStateUpdateGround(Animator animator) 
    {
        ;
    }

    virtual protected void OnStateEnterGround(Animator animator)
    {
        ;
    }

    virtual protected void OnStateOutGround(Animator animator)
    {
        ;
    }
}
