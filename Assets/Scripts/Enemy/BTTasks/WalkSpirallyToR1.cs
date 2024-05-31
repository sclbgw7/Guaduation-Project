using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Before attacking, Walk Spirally To MC. Stop when reach r1. Fail when no space for walking spirally.")]
    [TaskCategory("Enemy Action")]
    public class WalkSpirallyToR1 : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            // Debug.Log("walk start");
            self = GetComponent<Enemy>();
            self.WalkSpirallyToR1_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.WalkSpirallyToR1_OnUpdate();
        }

        public override void OnEnd()
        {
            // Debug.Log("walk end");
            self.WalkSpirallyToR1_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public void WalkSpirallyToR1_OnStart()
    {
        updateTasks += UpdateDestination_WalkSpirallyToR1;

        failFlag_Reuse = false;

        agent.speed = 1f;
        UpdateDestination_WalkSpirallyToR1();

        AnimTransferTo(WalkName);
    }

    public TaskStatus WalkSpirallyToR1_OnUpdate()
    {
        if (failFlag_Reuse)
        {
            return TaskStatus.Failure;
        }
        if (disConfront > r1)
        {
            Vector3 move = agent.nextPosition - transform.position;
            WalkWhileFaceToMC(move);

            return TaskStatus.Running;
        }
        // Debug.Log(disConfront);
        return TaskStatus.Success;
    }

    public void WalkSpirallyToR1_OnEnd()
    {
        updateTasks -= UpdateDestination_WalkSpirallyToR1;
    }

    protected void UpdateDestination_WalkSpirallyToR1()
    {
        Vector3 dis = (EnemyUtility.GetMCPosition() - transform.position);
        dis.y = 0;
        dis.Normalize();

        Vector3 preferedPos = transform.position + dis + Vector3.Cross(Vector3.up, dis);

        if (NavMesh.Raycast(transform.position, preferedPos, out NavMeshHit hit, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
            failFlag_Reuse = true;
        }
        else
        {
            agent.destination = preferedPos;
        }
    }
}