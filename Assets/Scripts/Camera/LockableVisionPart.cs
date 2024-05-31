using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableVisionPart : MonoBehaviour
{
    [SerializeField]
    private LockableTarget target;

    void OnBecameVisible()
    {
        LockedTargetManager.Instance.AddTarget(target);
    }

    void OnBecameInvisible()
    {
        LockedTargetManager.Instance.RemoveTarget(target);
    }
}
