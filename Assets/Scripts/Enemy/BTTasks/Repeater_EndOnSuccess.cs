namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription(@"The repeater task will repeat execution of its child task until the child task has been run a specified number of times. " +
                      "It has the option of continuing to execute the child task even if the child task returns a success.")]
    [TaskIcon("{SkinColor}RepeaterIcon.png")]
    public class Repeater_EndOnSuccess : Decorator
    {
        [Tooltip("The number of times to repeat the execution of its child task")]
        public SharedInt count = 1;
        [Tooltip("Allows the repeater to repeat forever")]
        public SharedBool repeatForever;
        [Tooltip("Should the task return if the child task returns a success")]
        public SharedBool endOnSuccess;

        // The number of times the child task has been run.
        private int executionCount = 0;
        // The status of the child after it has finished running.
        private TaskStatus executionStatus = TaskStatus.Inactive;

        public override bool CanExecute()
        {
            // Continue executing until we've reached the count or the child task returned success and we should stop on a success.
            return (repeatForever.Value || executionCount < count.Value) && (!endOnSuccess.Value || (endOnSuccess.Value && executionStatus != TaskStatus.Success));
        }

        public override void OnChildExecuted(TaskStatus childStatus)
        {
            // The child task has finished execution. Increase the execution count and update the execution status.
            executionCount++;
            executionStatus = childStatus;
        }

        public override void OnEnd()
        {
            // Reset the variables back to their starting values.
            executionCount = 0;
            executionStatus = TaskStatus.Inactive;
        }

        public override void OnReset()
        {
            // Reset the public properties back to their original values.
            count = 0;
            endOnSuccess = true;
        }
    }
}