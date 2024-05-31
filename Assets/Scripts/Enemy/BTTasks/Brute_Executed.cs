using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Being executed.")]
    [TaskCategory("Enemy Action")]
    public class Brute_Executed : Action
    {
        private Brute self;

        public override void OnStart()
        {
            self = GetComponent<Brute>();
            self.Executed_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.Executed_OnUpdate();
        }

        public override void OnEnd()
        {
            self.Executed_OnEnd();
        }
    }
}


public partial class Brute : Enemy
{
    [SerializeField]
    private AnimationCurve executedCurve;
    private StatedCurve statedCurve;
    private float lastAnimStartTime;

    public void Executed_OnStart()
    {
        executionStage = 0;
        // AnimTransferTo("Hurt Walk Back");

        startTime_Reuse = Time.time;
        statedCurve = new StatedCurve(executedCurve);
        statedCurve.EvaluateDelta(0f);

        Vector3 faceDir = EnemyUtility.GetMCPosition() - transform.position;
        faceDir.y = 0f;
        nowForward = faceDir.normalized;
    }

    public TaskStatus Executed_OnUpdate()
    {
        characterController.Move(-nowForward * statedCurve.EvaluateDelta((Time.time - startTime_Reuse)));

        if (executionStepTrigger)
        {
            executionStepTrigger = false;
            if (executionStage == 0)
            {
                AnimTransferTo("Execution Hurt 1");
            }
            else if (executionStage == 1)
            {
                AnimTransferTo("Execution Hurt 2");
            }
            else if (executionStage == 2)
            {
                AnimTransferTo("Die");
                // AnimTransferTo("Hurt Walk Back");
                lastAnimStartTime = Time.time;
            }
            else
            {
                throw new System.InvalidOperationException("Invalid execution stage number!");
            }
            executionStage++;
            return TaskStatus.Running;
        }
        // if (executionStage == 3 && Time.time - lastAnimStartTime >= 1.3667f)
        if (executionStage == 3 && Time.time - lastAnimStartTime >= 2.6f)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    public void Executed_OnEnd()
    {
        ;
    }
}