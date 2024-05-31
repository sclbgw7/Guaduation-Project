using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Before attacking, Walk directly To MC. Stop when reach r1.")]
    [TaskCategory("Enemy Action")]
    public class WalkToR1 : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.WalkToR1_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.WalkToR1_OnUpdate();
        }

        public override void OnEnd()
        {
            self.WalkToR1_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public void WalkToR1_OnStart()
    {
        updateTasks += UpdateDestination_ToMC;

        agent.speed = 1f;
        UpdateDestination_ToMC();

        AnimTransferTo(WalkName);
    }

    public TaskStatus WalkToR1_OnUpdate()
    {
        if (disConfront > r1)
        {
            Vector3 move = agent.nextPosition - transform.position;
            WalkWhileFaceToMC(move);

            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    public void WalkToR1_OnEnd()
    {
        updateTasks -= UpdateDestination_ToMC;
    }
}