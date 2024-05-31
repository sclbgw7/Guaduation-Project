using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCharacterAnimParameters
{
    // character to animator
    public bool isGround = true;
    public bool shouldFall = false; // high enough to transfer to InAir
    public float speed = 0.0f;

    // animator to characterMovement
    public bool canMove = true;
    public bool shouldJump = false;
    public bool shouldRoll = false;
    public bool shouldUpdateForward = false;
    public bool isAttacking = false;
    public bool isJumping = false; // is the nowState JumpBegin or JumpAir
    public float forwardMoveDeltaDis = 0.0f;
    public float moveScaleWhenLocking = 1.0f;
    public float maxMoveDis = 0.0f; // not used yet

    // animator to characterBattle
    public bool executing = false;
    public bool bladeIsGleaming = false; /* Is the attack in its detection window?
                                          * This is set in AttackCoroutine in MainCharacterBattle,
                                          * and is set to false again in StateOut in the attack state.
                                          */

    // not public
    public MainCharaStateAny nowState;

    // character's events
    public event Action applyStateMovement = delegate { };
    public void ApplyStateMovementToCharacter()
    {
        applyStateMovement();
    }

    public event Action<BladeAttackParameters, float, float> addAttackEvent = delegate { };
    public void AddAttackEvent(BladeAttackParameters parameters, float timeL, float timeR)
    {
        addAttackEvent(parameters, timeL, timeR);
    }

    public event Action<int> beginExecutionEvent = delegate { };
    public void BeginExecution(int executionID)
    {
        beginExecutionEvent(executionID);
    }

    public event Action endExecutionEvent = delegate { };
    public void EndExecution()
    {
        endExecutionEvent();
    }

    public event Action singleAttackOut = delegate { };
    public void SingleAttackOut()
    {
        singleAttackOut();
    }

    // anim triggers
    public event Func<Animator, bool> jumpTrigger = (_) => { return false; };
    public bool SetTriggerJump(Animator animator)
    {
        return jumpTrigger(animator);
    }

    public event Func<Animator, bool> fire1Trigger = (_) => { return false; };
    public bool SetTriggerFire1(Animator animator)
    {
        return fire1Trigger(animator);
    }

    public event Func<Animator, bool> rollTrigger = (_) => { return false; };
    public bool SetTriggerRoll(Animator animator)
    {
        return rollTrigger(animator);
    }

    public event Action<Animator> counterTrigger = delegate { };
    public void SetTriggerCounter(Animator animator)
    {
        counterTrigger(animator);
    }

    // instant methods
    public void ApplyHurt(Animator animator, int knockForce, Vector3 localHurtDir, out StateMachineHurtResult result)
    {
        nowState.Hurt(animator, knockForce, localHurtDir, out result);
    }
}
