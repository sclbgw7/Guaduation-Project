using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Perform an attack, let itself decides which attack to perform")]
    [TaskCategory("Enemy Action")]
    public class PerformAttack : Action
    {
        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.PerformAttack_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.PerformAttack_OnUpdate();
        }

        public override void OnEnd()
        {
            // Debug.Log("walk end");
            self.PerformAttack_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    // Those methods might be overrided!
    virtual public void PerformAttack_OnStart()
    {
        nowAttackTask = GetTask(out int attackIndex);

        startTime_Reuse = Time.time;
        duration_Reuse = nowAttackTask.totalFrame / 30.0f;
        
        EnemyUtility.StartAttackTask(nowAttackTask, this);

        AnimTransferTo(AttackName(attackIndex));
    }

    virtual public TaskStatus PerformAttack_OnUpdate()
    {
        if (Time.time - startTime_Reuse <= duration_Reuse)
        {
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

    virtual public void PerformAttack_OnEnd()
    {
        EnemyUtility.StopAttackTask(nowAttackTask);
    }

    abstract public EnemyAttackTask GetTask(out int attackIndex);
}