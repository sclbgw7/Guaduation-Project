using UnityEngine;

public class MainCharaStateHitLight : MainCharaStateAny
{
    [SerializeField]
    float rollFrame;
    [SerializeField]
    AnimationCurve moveCurve;

    private float exitTime;

    private StatedCurve statedCurve;


    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        rollBackswing = rollFrame / 30.0f;

        statedCurve = new StatedCurve(moveCurve);
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        ApplyMoveCurve();

        if (inStateTime >= exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    private void ApplyMoveCurve()
    {
        // move backwards
        parameters.forwardMoveDeltaDis -= statedCurve.EvaluateDelta(inStateTime * 30f);
    }
}