using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Large hurt + walk back.")]
    [TaskCategory("Enemy Action")]
    public class Brute_HurtLarge : Action
    {
        private Brute self;

        public override void OnStart()
        {
            self = GetComponent<Brute>();
            self.HurtLarge_OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            return self.HurtLarge_OnUpdate();
        }
    }
}


public partial class Brute : Enemy
{
    public void HurtLarge_OnStart()
    {
        startTime_Reuse = Time.time;
        AnimTransferTo("Hurt Walk Back");

        Vector3 faceDir = EnemyUtility.GetMCPosition() - transform.position;
        faceDir.y = 0f;
        nowForward = faceDir.normalized;
    }

    public TaskStatus HurtLarge_OnUpdate()
    {
        if (Time.time - startTime_Reuse >= 1.3667f)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}