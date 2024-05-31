using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("A form of random walking")]
    [TaskCategory("Enemy Action")]
    public class WalkLaterally : Action
    {
        private Enemy self;
        

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.WalkLaterally_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.WalkLaterally_OnUpdate();
        }

        public override void OnEnd()
        {
            self.WalkLaterally_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public void WalkLaterally_OnStart()
    {
        startTime_Reuse = Time.time;

        agent.speed = 1f;
        
        AnimTransferTo(WalkName);

        float walkDeg = (Random.Range(0, 2) == 0 ? -1f : 1f) * 60f;
        Vector3 dis = (transform.position - EnemyUtility.GetMCPosition()).normalized;

        dis = Quaternion.AngleAxis(walkDeg, Vector3.up) * dis;

        float preferedDis = (r2 + r1) * 0.5f + Random.Range(0f, 1f) * (r3 - r1) * 0.5f;
        // Debug.Log(preferedDis);

        Vector3 preferedPos =
            EnemyUtility.GetMCPosition() +
            dis * (preferedDis);

        if (NavMesh.Raycast(transform.position, preferedPos, out NavMeshHit hit, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
        else
        {
            agent.destination = preferedPos;
        }
    }

    public TaskStatus WalkLaterally_OnUpdate()
    {
        if (Time.time - startTime_Reuse > 20.0f)
        {
            Debug.Log("Walk Laterally: Time Limit!");
            return TaskStatus.Success;
        }
        if (!AgentReachedDestination())
        {
            Vector3 move = agent.nextPosition - transform.position;
            WalkWhileFaceToMC(move);

            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    public void WalkLaterally_OnEnd()
    {
        // StopRunning();
    }
}