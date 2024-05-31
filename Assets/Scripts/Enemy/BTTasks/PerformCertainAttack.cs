using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Perform a certain attack")]
    [TaskCategory("Enemy Action")]
    public class PerformCertainAttack : Action
    {
        [Tooltip("The index of this attack")]
        public int attackIndex;

        private Enemy self;

        public override void OnStart()
        {
            self = GetComponent<Enemy>();
            self.PerformCertainAttack_OnStart(attackIndex);
        }

        public override TaskStatus OnUpdate()
        {
            return self.PerformCertainAttack_OnUpdate();
        }

        public override void OnEnd()
        {
            // Debug.Log("walk end");
            self.PerformCertainAttack_OnEnd();
        }
    }
}


public abstract partial class Enemy : LockableTarget
{
    // Those methods might be overrided!
    virtual public void PerformCertainAttack_OnStart(int attackIndex)
    {
        nowAttackTask = GetCertainTask(attackIndex);

        startTime_Reuse = Time.time;
        duration_Reuse = nowAttackTask.totalFrame / 30.0f;
        
        EnemyUtility.StartAttackTask(nowAttackTask, this);

        AnimTransferTo(AttackName(attackIndex));
        // animator.CrossFadeInFixedTime($"Attack {attackIndex}", 0.25f);
    }

    virtual public TaskStatus PerformCertainAttack_OnUpdate()
    {
        return PerformAttack_OnUpdate();
    }

    virtual public void PerformCertainAttack_OnEnd()
    {
        PerformAttack_OnEnd();
    }

    abstract public EnemyAttackTask GetCertainTask(int index);
}