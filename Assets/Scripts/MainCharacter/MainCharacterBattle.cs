using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct BladeAttackParameters
{
    public float damage;
    public int breakForce;

    // for camera shaking
    public Vector3 localDirection;
    public float shakeForce;
}

[System.Serializable]
public struct ExecutionAttackParameters
{
    public float frame;
    public float damage;

    // for camera shaking
    public Vector3 localDirection;
    public float shakeForce;
}

[System.Serializable]
public class ExecutionInfo
{
    public List<ExecutionAttackParameters> attackEvents;
    public float endFrame;
    public AnimationCurve dashCurve;
    public float dashEndDisFromEnemy;
    public AnimationCurve cameraDollyCurve;
    public DollyCameraSet cameraDollySet;
}

public class MainCharacterBattle : MonoBehaviour
{
    [SerializeField]
    private CinemachineImpulseSource bladeImpulse;
    [SerializeField]
    private CinemachineImpulseSource hurtImpulse;
    [SerializeField]
    private CinemachineImpulseSource reboundImpulse;
    [SerializeField]
    private BoxCollider bladeCollider;
    [SerializeField]
    private LayerMask hurtableLayer;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private List<ExecutionInfo> executions;

    private MainCharacterAnimParameters animParameters;
    [SerializeField]
    private Animator animator;
    // use this as few as you can
    private MainCharacterMovement movementComponent;

    private Enemy nowCounterEnemy;
    private ExecutionInfo nowExecution;
    private int playerLayerIndex;
    private int hurtableLayerIndex;

    // Start is called before the first frame update
    void Start()
    {
        movementComponent = GetComponent<MainCharacterMovement>();

        animParameters = movementComponent.animParameters;
        animParameters.addAttackEvent += this.AddAttackEvent;
        animParameters.singleAttackOut += this.ClearAttackEvents;
        animParameters.beginExecutionEvent += this.BeginCounterExecution;
        animParameters.endExecutionEvent += this.EndExecution;

        playerLayerIndex = LayerMask.NameToLayer("Player");
        hurtableLayerIndex = LayerMask.NameToLayer("Hurtable");

        // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.H))
        {
            // animParameters.nowState.Hurt(animator, 1, new Vector3(0f, 0f, -1f));
            animParameters.SetTriggerCounter(animator);
        }
    }

    private void AddAttackEvent(BladeAttackParameters parameters, float timeL, float timeR)
    {
        StartCoroutine(AttackCoroutine(parameters, timeL, timeR - timeL));
    }

    private void ClearAttackEvents()
    {
        StopAllCoroutines();
    }

    IEnumerator AttackCoroutine(BladeAttackParameters parameters, float time, float lasting)
    {
        yield return new WaitForSeconds(time);

        Vector3 shakeDir = parameters.localDirection * parameters.shakeForce;
        shakeDir = bladeImpulse.transform.rotation * shakeDir;

        // It's not a hash set but I think the collection will be very small
        // so it might be faster.
        List<HurtableObject> hitHashSet = new List<HurtableObject>();

        // Use this to reduce code duplication
        // Captured variables: hitHashSet, bladeImpulse, shakeDir
        Action<HurtableObject> hitTargetLambda = (HurtableObject target) => 
        {
            if (target != null && !hitHashSet.Contains(target))
            {
                HurtParam hurtParam = new HurtParam
                {
                    damage = 0f,
                    knockForce = 1,
                    dir = target.transform.position - transform.position,
                };
                HurtResult hurtResult = target.Hurt(hurtParam);

                if (hurtResult.isCounter)
                {
                    Debug.Log("Counter!");
                    animParameters.SetTriggerCounter(animator);
                    reboundImpulse.GenerateImpulse(2.0f);

                    CounterArea counterArea = target as CounterArea;
                    if (counterArea != null)
                    {
                        nowCounterEnemy = counterArea.GetOwner();
                    }
                    else
                    {
                        throw new System.InvalidOperationException("Counter triggered without a CounterArea!");
                    }
                }

                if (hitHashSet.Count == 0)
                {
                    bladeImpulse.GenerateImpulseWithVelocity(shakeDir * hurtResult.shakeFactor);
                }
                hitHashSet.Add(target);
            }
        };

        Vector3 lastWorldCenter = bladeCollider.transform.TransformPoint(bladeCollider.center);
        float beginTime = Time.time;

        while (Time.time - beginTime <= lasting)
        {
            animParameters.bladeIsGleaming = true;

            Vector3 worldCenter = bladeCollider.transform.TransformPoint(bladeCollider.center);
            Vector3 worldHalfExtents = Vector3.Scale(bladeCollider.size, bladeCollider.transform.lossyScale) * 0.5f;

            Vector3 castDir = lastWorldCenter - worldCenter;
            if (castDir.magnitude < 0.01f)
            {
                Collider[] hitColliders = Physics.OverlapBox(
                    worldCenter, 
                    worldHalfExtents, 
                    bladeCollider.transform.rotation, 
                    hurtableLayer);

                foreach (Collider hitCollider in hitColliders)
                {
                    HurtableObject target = hitCollider.GetComponent<HurtableObject>();
                    hitTargetLambda(target);
                }
            }
            else
            {
                RaycastHit[] hits = Physics.BoxCastAll(
                    worldCenter,
                    worldHalfExtents,
                    castDir.normalized,
                    bladeCollider.transform.rotation,
                    castDir.magnitude,
                    hurtableLayer);

                foreach (RaycastHit hit in hits)
                {
                    HurtableObject target = hit.collider.GetComponent<HurtableObject>();
                    hitTargetLambda(target);
                }
            }
            
            lastWorldCenter = worldCenter;
            yield return null;
        }

        animParameters.bladeIsGleaming = false;

        /*
        if (hitHashSet.Count > 0)
        {
            Debug.Log("Hit!");
        }
        else
        {
            Debug.Log("Miss!");
        }
        */
    }

    private void BeginCounterExecution(int executionID)
    {
        ExecutionInfo executionInfo = executions[executionID - 1];
        nowExecution = executionInfo;

        StartCoroutine(CounterExecutionCoroutine());
        StartCoroutine(ExecutionDashCoroutine());
        StartCoroutine(SetCameraDollyCoroutine());
    }

    IEnumerator CounterExecutionCoroutine()
    {
        Physics.IgnoreLayerCollision(playerLayerIndex, hurtableLayerIndex, true);
        movementComponent.UpdateForwardTowards(nowCounterEnemy.transform.position);
        
        nowCounterEnemy.EnterExecution();

        float startTime = Time.time;

        foreach (ExecutionAttackParameters attackParam in nowExecution.attackEvents)
        {
            yield return new WaitForSeconds(attackParam.frame / 30.0f - (Time.time - startTime));

            nowCounterEnemy.StepExecution();

            Vector3 shakeDir = attackParam.localDirection * attackParam.shakeForce;
            shakeDir = bladeImpulse.transform.rotation * shakeDir;
            bladeImpulse.GenerateImpulseWithVelocity(shakeDir);
        }
    }

    IEnumerator ExecutionDashCoroutine()
    {
        StatedCurve curve = new StatedCurve(nowExecution.dashCurve);
        float startTime = Time.time;
        float startDashTime = nowExecution.dashCurve.keys[0].time;
        float endDashTime = nowExecution.dashCurve.keys[^1].time;
        
        
        yield return new WaitForSeconds(startDashTime / 30.0f);
        Vector3 nowDis = nowCounterEnemy.transform.position - transform.position;
        nowDis.y = 0f;
        float scale = (nowDis.magnitude - nowExecution.dashEndDisFromEnemy) / nowExecution.dashCurve.keys[^1].value;

        while (Time.time - startTime < endDashTime)
        {
            animParameters.forwardMoveDeltaDis += curve.EvaluateDelta((Time.time - startTime) * 30.0f) * scale;
            animParameters.ApplyStateMovementToCharacter();
            yield return null;
        }
    }

    IEnumerator SetCameraDollyCoroutine()
    {
        nowExecution.cameraDollySet.Reset();
        nowExecution.cameraDollySet.SetActive(true);

        float startTime = Time.time;
        float endTime = nowExecution.endFrame / 30.0f;

        while (Time.time - startTime < endTime)
        {
            nowExecution.cameraDollySet.SetPosition(nowExecution.cameraDollyCurve.Evaluate((Time.time - startTime) * 30.0f));
            // Debug.Log(nowExecution.cameraDollyCurve.Evaluate((Time.time - startTime) * 30.0f));
            yield return null;
        }
    }

    private void EndExecution()
    {
        Physics.IgnoreLayerCollision(playerLayerIndex, hurtableLayerIndex, false);
        nowExecution.cameraDollySet.SetActive(false);
    }

    public void Hurt(EnemyAttackEvent hurtEvent, Vector3 shakeDir, Enemy enemy)
    {
        // Debug.Log(shakeDir);

        Vector3 hurtDir = (transform.position - enemy.transform.position).normalized;
        float f = Vector3.Dot(transform.forward, hurtDir);
        float r = Vector3.Dot(Vector3.Cross(Vector3.up, transform.forward), hurtDir);

        animParameters.ApplyHurt(animator, hurtEvent.breakForce, new Vector3(r, 0f, f), out StateMachineHurtResult hurtResult);

        if (hurtResult.shakeCamera)
        {
            hurtImpulse.GenerateImpulseWithVelocity(shakeDir);
        }
        if (hurtResult.applyDamage)
        {
            // Calculate the damage...
            ;
        }
        if (hurtResult.changeDirToKnockback)
        {
            movementComponent.UpdateForwardWhenHit(hurtDir);
        }
    }

    public bool BladeIsGleaming()
    {
        return animParameters.bladeIsGleaming; 
        // return animParameters.isAttacking;
    }
}
