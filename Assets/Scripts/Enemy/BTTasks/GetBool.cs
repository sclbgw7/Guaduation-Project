using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Get a bool value.")]
    [TaskCategory("Custom")]
    public class GetBool : Conditional
    {
        [Tooltip("The bool value you want to get")]
        public SharedBool theValue;
        [Tooltip("Invert the bool")]
        public bool invert;

        public override TaskStatus OnUpdate()
        {
            return (theValue.Value ^ invert) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}