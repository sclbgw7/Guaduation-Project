using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockableTarget : HurtableObject
{
    public abstract Vector3 GetLockingPosition();
}
