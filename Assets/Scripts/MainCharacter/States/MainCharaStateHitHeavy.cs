using UnityEngine;

public class MainCharaStateHitHeavy : MainCharaStateAny
{
    [SerializeField]
    AnimationCurve moveCurve;

    private float exitTime;

    private StatedCurve statedCurve;


    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        statedCurve = new StatedCurve(moveCurve);
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        ApplyMoveCurve();

        if (inStateTime >= exitTime)
        {
            TransferTo(animator, "Lying");
        }
    }

    private void ApplyMoveCurve()
    {
        // move backwards
        parameters.forwardMoveDeltaDis -= statedCurve.EvaluateDelta(inStateTime * 30f);
    }

    override protected bool CanRoll()
    {
        return false;
    }
}