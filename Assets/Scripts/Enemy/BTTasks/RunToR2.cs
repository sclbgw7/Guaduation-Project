using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Run to the r2 from the outside. Stop when reach r2.")]
    [TaskCategory("Enemy Action")]
    public class RunToR2 : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.RunToR2_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.RunToR2_OnUpdate();
        }

        public override void OnEnd()
        {
            self.RunToR2_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public void RunToR2_OnStart()
    {
        updateTasks += UpdateDestination_ToMC;

        agent.speed = 3f;
        smoothMove = transform.forward.normalized * smoothMove.magnitude;

        AnimTransferTo(UnarmedRunName);
        animator.SetFloat(Run_Speed_Name, 1.0f);

        agent.destination = EnemyUtility.GetMCPosition();
    }

    public TaskStatus RunToR2_OnUpdate()
    {
        if (disConfront > r2)
        {
            Vector3 move = agent.nextPosition - transform.position;
            characterController.Move(move);

            move.y = 0f;
            float smoothLambda = Mathf.Min(1.0f, Time.deltaTime / 0.1f);
            smoothMove = Vector3.Lerp(smoothMove, move, smoothLambda);

            if (move.magnitude >= 0.0001f)
            {
                animator.SetFloat(Run_Speed_Name, move.magnitude / (agent.speed * Time.deltaTime));

                if (smoothMove.magnitude > 0.005f)
                {
                    nowForward = smoothMove.normalized;
                }
            }
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    public void RunToR2_OnEnd()
    {
        updateTasks -= UpdateDestination_ToMC;

        // StopRunning();
    }
}