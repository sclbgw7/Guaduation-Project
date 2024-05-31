using BehaviorDesigner.Runtime.Tasks.Unity.UnityRenderer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainCharaStateAny : StateMachineBehaviour
{
    // should be initialized -------
    [SerializeField]
    protected float rollBackswing = -1.0f; // after this time, character can roll
    [SerializeField]
    protected int endure = -1;
    // -----------------------------

    protected MainCharacterAnimParameters parameters;
    protected AnimatorStateInfo stateInfo { get; private set; }
    protected int layerIndex { get; private set; }
    protected float inStateTime { get; private set; }

    private float stateBeginTime;
    private bool isOut;
    private bool initialized = false;

    // override sealed public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    private void Initialize(Animator animator)
    {
        InitializeAny();
        Debug.Assert(rollBackswing >= 0.0f && endure >= 0);

        parameters = animator.transform.parent.GetComponent<MainCharacterMovement>().animParameters;
    }

    override sealed public void OnStateEnter(Animator animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        if(!initialized)
        {
            initialized = true;
            Initialize(animator);
        }

        parameters.nowState = this;
        stateInfo = _stateInfo;
        layerIndex = _layerIndex;

        inStateTime = 0.0f;
        stateBeginTime = Time.time;
        isOut = false;

        parameters.jumpTrigger += this.OnTriggerJump;
        parameters.rollTrigger += this.OnTriggerRoll;

        OnStateEnterAny(animator);
    }

    override sealed public void OnStateUpdate(Animator animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        inStateTime = Time.time - stateBeginTime;

        if(isOut)
        {
            return;
        }

        OnStateUpdateAny(animator);

        /* The the state machine is updated after the character.
         * In some states we have functions applying movement to character, which is based on the deltaTime.
         * If we defer the movement to the next frame, flickering of the camera will occur.
         * So the movement must be applied in this frame.
         */
        parameters.ApplyStateMovementToCharacter();
    }

    override sealed public void OnStateExit(Animator animator, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        ;   // shouldn't use this, use OnStateOut instead
    }

    private void OnStateOut(Animator animator)
    {
        // Maybe it's not needed, instead register these triggers once and never unregiser it
        parameters.jumpTrigger -= this.OnTriggerJump;
        parameters.rollTrigger -= this.OnTriggerRoll;
        OnStateOutAny(animator);
    }
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    // interfaces  ----------------------------------------

    virtual protected void InitializeAny()
    {
        ;
    }

    virtual protected void OnStateEnterAny(Animator animator)
    {
        ;
    }

    virtual protected void OnStateUpdateAny(Animator animator)
    {
        ;
    }

    virtual protected void OnStateOutAny(Animator animator)
    {
        ;
    }

    virtual protected bool CanRoll()
    {
        return true;
    }

    virtual protected bool IsInvincible()
    {
        return false;
    }

    virtual protected bool IsUnhurtable()
    {
        return false;
    }

    // methods --------------------------------------------

    public void Hurt(Animator animator, int knockForce, Vector3 localHurtDir, out StateMachineHurtResult result)
    {
        result = new StateMachineHurtResult
        {
            applyDamage = false,
            shakeCamera = false,
            changeDirToKnockback = false,
        };

        if (IsInvincible())
        {
            return;
        }
        result.shakeCamera = true;

        if (IsUnhurtable())
        {
            return;
        }
        result.applyDamage = true;

        if (knockForce <= endure)
        {
            // Debug.Log(localHurtDir);
            animator.SetTrigger("Apply_Additive_Hurt");
            animator.SetFloat("Hurt_Blend_R", localHurtDir.x);
            animator.SetFloat("Hurt_Blend_F", localHurtDir.z);
        }
        else
        {
            result.changeDirToKnockback = true;

            if(knockForce <= 2) 
            {
                // light hurt
                TransferTo(animator, "Hit_Light", 0.1f);
            }
            else
            {
                // heavy hurt
                TransferTo(animator, "Hit_Heavy", 0.1f);
            }
        }
    }

    // methods for subclasses --------------------------------------------

    protected void TransferTo(Animator animator, string stateName, float transitionDuration = 0.25f)
    {
        isOut = true;
        OnStateOut(animator);

        animator.CrossFadeInFixedTime(stateName, transitionDuration);
    }

    protected void CommonEndingTransfer(Animator animator)
    {
        const float speedThresholdGreater = 0.1f;

        if (!parameters.isGround)
        {
            TransferTo(animator, "Jump_InAir");
        }
        else if(parameters.speed >= speedThresholdGreater)
        {
            TransferTo(animator, "Run");
        }
        else
        {
            TransferTo(animator, "Idle");
        }
    }

    protected void CommonMoveBackswingTransfer(Animator animator)
    {
        const float speedThresholdGreater = 0.1f;

        if (!parameters.isGround)
        {
            TransferTo(animator, "Jump_InAir");
        }
        else if (parameters.speed >= speedThresholdGreater)
        {
            TransferTo(animator, "Run");
        }
    }

    // common triggers -------------------------

    private bool OnTriggerJump(Animator animator)
    {
        if (parameters.isGround && (parameters.canMove || inStateTime >= rollBackswing))
        {
            // Debug.Log("Jumping!");
            parameters.shouldJump = true;
            TransferTo(animator, "Jump_Begin");
            return true;
        }
        return false;
    }

    private bool OnTriggerRoll(Animator animator)
    {
        if (CanRoll() && inStateTime >= rollBackswing)
        {
            // Debug.Log("Rolling!");
            TransferTo(animator, "Roll");
            return true;
        }
        return false;
    }
}

public struct StateMachineHurtResult
{
    public bool applyDamage;
    public bool shakeCamera;
    public bool changeDirToKnockback;
}
