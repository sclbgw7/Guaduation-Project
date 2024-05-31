using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterArea : HurtableObject
{
    [SerializeField]
    protected Enemy owner;

    public override HurtResult Hurt(HurtParam param)
    {
        param.knockForce = 3;
        owner.Hurt(param);

        return new HurtResult
        {
            succeed = true,
            isCounter = true,
            shakeFactor = 1.0f,
        };
    }

    public Enemy GetOwner()
    {
        return owner;
    }
}
