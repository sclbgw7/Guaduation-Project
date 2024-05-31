using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Set a bool value.")]
    [TaskCategory("Custom")]
    public class SetBool : Action
    {
        [Tooltip("The bool value you want to set")]
        public SharedBool theValue;
        [Tooltip("The desired value")]
        public bool setTo;

        public override TaskStatus OnUpdate()
        {
            theValue.Value = setTo;
            return TaskStatus.Success;
        }
    }
}