using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTarget : LockableTarget
{
    public override Vector3 GetLockingPosition()
    {
        return transform.position;
    }

    public override HurtResult Hurt(HurtParam _)
    {
        // Debug.Log("Hurt!");

        return new HurtResult
        {
            succeed = true,
            shakeFactor = 1.0f,
        };
    }
}
