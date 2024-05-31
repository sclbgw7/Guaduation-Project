using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    static public void SimpleTweenFloat(ref float source, float target, float duration)
    {
        // ATTENTION: duration is not the exact duration of this tweening
        float moveStride = Time.deltaTime / duration;
        if (Mathf.Abs(target - source) > 1f)
        {
            source += (target - source) * moveStride;
        }
        else if (Mathf.Abs(target - source) > moveStride)
        {
            source += Mathf.Sign(target - source) * moveStride;
        }
        else
        {
            source = target;
        }
    }
}
