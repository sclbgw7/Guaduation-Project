using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharaStateRebound : MainCharaStateAny
{
    [SerializeField]
    float continueFrame;
    [SerializeField]
    AnimationCurve moveCurve;
    [SerializeField]
    AnimationCurve timeScaleCurve;
    DollyCameraSet reboundCameraSet;
    [SerializeField]
    AnimationCurve reboundDollyCurve;

    private float exitTime;
    private float continueTime;

    private StatedCurve statedCurve;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        continueTime = continueFrame / 120.0f;

        parameters.fire1Trigger += this.OnTriggerFire1;

        statedCurve = new StatedCurve(moveCurve);

        // You can't configure CameraSet in animator directly,
        // because animator is not scene-specific.
        // so you must get that from an object in the scene.
        reboundCameraSet = CameraManager.Instance.GetReboundCameraSet();
        reboundCameraSet.Reset();
        reboundCameraSet.SetActive(true);
    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        parameters.forwardMoveDeltaDis += statedCurve.EvaluateDelta(inStateTime/* * 120f */);

        Time.timeScale = timeScaleCurve.Evaluate(inStateTime * 120.0f);
        reboundCameraSet.SetPosition(reboundDollyCurve.Evaluate(inStateTime * 120.0f));

        if (inStateTime > exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.fire1Trigger -= this.OnTriggerFire1;

        Time.timeScale = 1.0f;
        reboundCameraSet.SetActive(false);
    }

    private bool OnTriggerFire1(Animator animator)
    {
        if (inStateTime >= continueTime)
        {
            TransferTo(animator, "Counter_Execution");
            return true;
        }
        return false;
    }

    override protected bool CanRoll()
    {
        return false;
    }

    override protected bool IsInvincible()
    {
        return true;
    }
}
