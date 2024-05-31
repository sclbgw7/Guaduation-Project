using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Stop the Navigation.")]
    [TaskCategory("Enemy Action")]
    public class ClearNavigation : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
        }

        public override TaskStatus OnUpdate()
        {
            return self.ClearNavigation_OnUpdate();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    public TaskStatus ClearNavigation_OnUpdate()
    {
        agent.destination = transform.position;
        agent.nextPosition = transform.position;

        return TaskStatus.Success;
    }
}