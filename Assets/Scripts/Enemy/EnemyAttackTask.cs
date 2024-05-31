using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyAttackParam
{
    public BoxCollider weaponArea;

    public float attackFrameL;
    public float attackFrameR;

    public float damage;
    public int breakForce;

    // for camera shaking
    [Tooltip("The force direction relative to the collider (Weapon Area).")]
    public Vector3 localShakeDirection;
    public float shakeForce;
}

[System.Serializable]
public struct EnemyCounterParam
{
    public CounterArea counterArea;

    public float counterFrameL;
    public float counterFrameR;
}

[System.Serializable]
public struct EnemyCounterableAttackEvent
{
    public EnemyAttackParam normalParameters;

    public EnemyCounterParam counterParameters;
}

[System.Serializable]
public class EnemyAttackTask
{
    public List<EnemyAttackParam> normalAttackEvents;
    public List<EnemyCounterableAttackEvent> counterableAttackEvents;
    public float totalFrame;
}

public struct EnemyAttackEvent
{
    public float damage;
    public int breakForce;
}
