using UnityEngine;

public class MainCharaStateLying : MainCharaStateAny
{
    [SerializeField]
    float exitTime;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        parameters.rollTrigger += this.OnTriggerMoving;
        parameters.jumpTrigger += this.OnTriggerMoving;
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        const float speedThresholdGreater = 0.1f;

        if (parameters.speed >= speedThresholdGreater)
        {
            TransferTo(animator, "Rise");
        }
        if (inStateTime > exitTime)
        {
            TransferTo(animator, "Rise");
        }
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.rollTrigger -= this.OnTriggerMoving;
        parameters.jumpTrigger -= this.OnTriggerMoving;
    }

    private bool OnTriggerMoving(Animator animator)
    {
        TransferTo(animator, "Rise");
        return true;
    }

    override protected bool CanRoll()
    {
        return false;
    }
}