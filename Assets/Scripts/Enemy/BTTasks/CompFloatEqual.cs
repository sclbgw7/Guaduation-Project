using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("Returns success if a >= b")]
    [TaskCategory("Custom")]
    public class CompFloatEqual : Conditional
    {
        [Tooltip("The reference of a")]
        public SharedFloat a;
        [Tooltip("The reference of b")]
        public SharedFloat b;
        [Tooltip("Returns success if a <= b")]
        public bool lessThan;

        public override TaskStatus OnUpdate()
        {
            if (!lessThan)
            {
                return a.Value >= b.Value ? TaskStatus.Success : TaskStatus.Failure;
            }
            else
            {
                return a.Value <= b.Value ? TaskStatus.Success : TaskStatus.Failure;
            }
        }
    }
}