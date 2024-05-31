using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Read properties from the enemy script.")]
    [TaskCategory("Custom")]
    public class ReadProperties : Action
    {
        public override TaskStatus OnUpdate()
        {
            Enemy self = GetComponent<Enemy>();
            // Log the text and return success
            if (self)
            {
                self.WritePropertiesToBehaviorTree();
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
            
        }
    }
}
