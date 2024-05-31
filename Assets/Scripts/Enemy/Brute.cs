using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public partial class Brute : Enemy
{
    [SerializeField]
    private List<EnemyAttackTask> AttackTasks;

    private float gravityValue = -9.81f;
    // private bool applyGravity = true;
    private float lastPosY;
    private int lastAttackTask1;
    private int lastAttackTask2;

    private int executionStage;
    private bool executionStepTrigger;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        lastPosY = transform.position.y;
        lastAttackTask1 = -1;
        lastAttackTask2 = -1;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    void OnAnimatorMove()
    {
        // animator.ApplyBuiltinRootMotion();
        Vector3 deltaPosition = animator.deltaPosition;

        
        deltaPosition.y = -deltaPosition.magnitude;

        float nowPosY = transform.position.y;
        deltaPosition.y += ((nowPosY - lastPosY) / Time.deltaTime + gravityValue * Time.deltaTime) * Time.deltaTime;
        lastPosY = nowPosY;

        // Debug.Log(deltaPosition);

        // forward motion to the parent object
        characterController.Move(deltaPosition);
    }

    public override Vector3 GetLockingPosition()
    {
        return transform.position + new Vector3(0f, 1.3f, 0f);
    }

    public override HurtResult Hurt(HurtParam param)
    {
        // Debug.Log("Hurt!");

        if (param.knockForce >= 3)
        {
            behaviorTree.SetVariableValue("hurt", true);
        }

        return new HurtResult
        {
            succeed = true,
            isCounter = false,
            shakeFactor = 0.5f,
        };
    }

    public override EnemyAttackTask GetCertainTask(int index)
    {
        return AttackTasks[index - 1];
    }

    public override EnemyAttackTask GetTask(out int attackIndex)
    {
        if (lastAttackTask1 == 2)
        {
            attackIndex = 1;
        }
        else if (lastAttackTask2 == 1)
        {
            attackIndex = 2;
        }
        else
        {
            attackIndex = Random.Range(1, 3);
        }

        lastAttackTask2 = lastAttackTask1;
        lastAttackTask1 = attackIndex;
        return AttackTasks[attackIndex - 1];
    }

    public override void StepExecution()
    {
        executionStepTrigger = true;
    }
}
