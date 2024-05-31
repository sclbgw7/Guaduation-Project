using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class MainCharaStateCounterExecution : MainCharaStateAny
{
    [SerializeField]
    float rollFrame;
    [SerializeField]
    float moveFrame;
    [SerializeField]
    AnimationCurve moveCurve;

    private float exitTime;
    private float moveBackswing;

    private StatedCurve statedCurve;

    override sealed protected void OnStateEnterAny(Animator animator)
    {
        exitTime = stateInfo.length;

        moveBackswing = moveFrame / 30.0f;
        rollBackswing = rollFrame / 30.0f;

        statedCurve = new StatedCurve(moveCurve);

        parameters.executing = true;

        parameters.BeginExecution(1);

        // Use this to extract root motion into AnimationCurve.
        // Woohoo!
        /*
        var temp = animator.GetNextAnimatorClipInfo(0);
        var curveBindings = UnityEditor.AnimationUtility.GetCurveBindings(temp[0].clip);
        foreach (var curveBinding in curveBindings)
        {
            if (curveBinding.propertyName == "RootT.z")
            {
                
                var editorCurve = UnityEditor.AnimationUtility.GetEditorCurve(temp[0].clip, curveBinding);
                moveCurve = editorCurve;

                Debug.Log(moveCurve.length);
            }
        }
        // Debug.Log(temp[0].clip.name);
        */

    }

    override sealed protected void OnStateUpdateAny(Animator animator)
    {
        parameters.forwardMoveDeltaDis += statedCurve.EvaluateDelta(inStateTime/* * 30f*/);

        if (inStateTime >= moveBackswing)
        {
            parameters.canMove = true;

            CommonMoveBackswingTransfer(animator);
        }
        if (inStateTime > exitTime)
        {
            CommonEndingTransfer(animator);
        }
    }

    override sealed protected void OnStateOutAny(Animator animator)
    {
        parameters.canMove = false;
        parameters.executing = false;

        parameters.EndExecution();
    }

    override protected bool IsInvincible()
    {
        return true;
    }
}
