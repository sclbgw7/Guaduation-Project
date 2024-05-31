using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Back To Idle.")]
    [TaskCategory("Enemy Action")]
    public class ToIdle : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            return self.ToIdle_OnUpdate();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public TaskStatus ToIdle_OnUpdate()
    {
        agent.destination = transform.position;
        AnimTransferTo(StandingIdleName);

        return TaskStatus.Success;
    }
}