using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("When Main Character is too close, walk back to r2. Stop when reach r2.")]
    [TaskCategory("Enemy Action")]
    public class WalkBackToR2 : Action
    {
        private Enemy self;
        
        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.WalkBackToR2_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.WalkBackToR2_OnUpdate();
        }

        public override void OnEnd()
        {
            self.WalkBackToR2_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public void WalkBackToR2_OnStart()
    {
        updateTasks += UpdateDestination_BackToR2;

        agent.speed = 1.5f;

        AnimTransferTo(WalkBackName);

        UpdateDestination_BackToR2();
    }

    public TaskStatus WalkBackToR2_OnUpdate()
    {
        if (disConfront < r2)
        {
            Vector3 move = agent.nextPosition - transform.position;
            characterController.Move(move);

            move.y = 0f;
            float smoothLambda = Mathf.Min(1.0f, Time.deltaTime / 0.1f);
            smoothMove = Vector3.Lerp(smoothMove, move, smoothLambda);

            if (move.magnitude >= 0.0001f)
            {
                if (smoothMove.magnitude > 0.005f)
                {
                    Vector3 faceDir = EnemyUtility.GetMCPosition() - transform.position;
                    faceDir.y = 0f;
                    nowForward = faceDir.normalized;
                }
            }
            return TaskStatus.Running;
        }
        
        return TaskStatus.Success;
    }

    public void WalkBackToR2_OnEnd()
    {
        updateTasks -= UpdateDestination_BackToR2;

        // StopRunning();
    }

    protected void UpdateDestination_BackToR2()
    {
        Vector3 preferedPos =
            EnemyUtility.GetMCPosition() +
            (transform.position - EnemyUtility.GetMCPosition()).normalized * r3;

        if (NavMesh.Raycast(transform.position, preferedPos, out NavMeshHit hit, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
        else
        {
            agent.destination = preferedPos;
        }
    }
}