using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatedCurve
{
    private AnimationCurve curve;
    private float lastValue;

    public StatedCurve(AnimationCurve curve)
    {
        this.curve = curve;
        this.lastValue = curve.keys[0].value;
    }

    public float EvaluateDelta(float x)
    {
        float newValue = curve.Evaluate(x);
        float ret = newValue - lastValue;
        lastValue = newValue;
        return ret;
    }
}
