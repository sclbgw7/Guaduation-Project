using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using System;

public abstract partial class Enemy : LockableTarget
{
    // TODO: Some protected should be changed to private later...

    [SerializeField]
    protected float r1;
    [SerializeField]
    protected float r2;
    [SerializeField]
    protected float r3;
    [SerializeField]
    protected float r4;
    [SerializeField]
    protected float r5;

    [SerializeField]
    protected float stateUpdateInterval = 0.2f;

    // anim parameter names ---------------------------
    // can be overrided
    protected virtual string Walk_Blend_R_Name => "Walk_Blend_R";
    protected virtual string Walk_Blend_F_Name => "Walk_Blend_F"; 
    protected virtual string Run_Speed_Name => "Run_Speed";
    protected virtual string WalkName => "Walk";
    protected virtual string StandingIdleName => "Standing Idle";
    protected virtual string UnarmedIdleName => "Unarmed Idle";
    protected virtual string StandingRunName => "Standing Run";
    protected virtual string UnarmedRunName => "Unarmed Run";
    protected virtual string WalkBackName => "Walk Back";
    protected virtual string AttackName(int index) => $"Attack {index}";

    // end anim parameter names -----------------------

    protected float disConfront;

    protected Vector3 smoothMove;
    protected Vector3 nowForward;
    protected float startTime_Reuse;
    protected float duration_Reuse;
    protected bool failFlag_Reuse;
    private EnemyAttackTask nowAttackTask;

    protected Animator animator;
    protected CharacterController characterController;
    protected NavMeshAgent agent;
    protected BehaviorTree behaviorTree;

    protected event Action updateTasks;

    // public states ----------------------------
    public bool counterAreaActive;

    // end public states ------------------------

    // Start is called before the first frame update
    protected void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<BehaviorTree>();

        agent.updatePosition = false;

        updateTasks = UpdateStates;
        StartCoroutine(UpdateStateCoroutine());

        smoothMove = Vector3.zero;
        nowForward = transform.forward;

        // Debug.Log("Enemy Start!");
    }

    protected void Update()
    {
        RotateModelToForward();
    }

    protected void RotateModelToForward()
    {
        float angleGap = Vector3.Angle(gameObject.transform.forward, nowForward) / 180.0f * Mathf.PI;
        float tLerp = 10f /*agent.angularSpeed*/ * Time.deltaTime / angleGap;
        
        if (tLerp >= 1)
        {
            gameObject.transform.forward = nowForward;
        }
        else
        {
            gameObject.transform.forward =
                Quaternion.Lerp(
                        Quaternion.LookRotation(gameObject.transform.forward),
                        Quaternion.LookRotation(nowForward),
                        tLerp
                    ) * Vector3.forward;
        }

    }

    IEnumerator UpdateStateCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(stateUpdateInterval);
        while (true)
        {
            updateTasks();
            yield return wait;
        }
    }

    protected void AnimTransferTo(string stateName, float duration = 0.25f)
    {
        animator.CrossFadeInFixedTime(stateName, duration);
    }

    protected void UpdateStates()
    {
        float disOnNavMesh = EnemyUtility.CalcDisToMCOnNavMesh(transform.position);
        float disEuclid = (EnemyUtility.GetMCPosition() - transform.position).magnitude;
        behaviorTree.SetVariableValue("disDiscover", disEuclid);
        if (disOnNavMesh > disEuclid * 1.414f)
        {
            disConfront = disOnNavMesh;
        }
        else
        {
            disConfront = disEuclid;
        }
        behaviorTree.SetVariableValue("disConfront", disConfront);

        behaviorTree.SendEvent("ReevaluateSuggestion");
    }

    public void WritePropertiesToBehaviorTree()
    {
        behaviorTree.SetVariableValue("stateUpdateInterval", stateUpdateInterval);

        behaviorTree.SetVariableValue("r1", r1);
        behaviorTree.SetVariableValue("r2", r2);
        behaviorTree.SetVariableValue("r3", r3);
        behaviorTree.SetVariableValue("r4", r4);
        behaviorTree.SetVariableValue("r5", r5);

        behaviorTree.SetVariableValue("inBattle", true);

        UpdateStates();
    }

    protected void UpdateDestination_ToMC()
    {
        agent.destination = EnemyUtility.GetMCPosition();
    }

    protected bool AgentReachedDestination()
    {
        return !agent.pathPending && agent.velocity.magnitude < 0.3f && agent.remainingDistance <= 0.05f;
    }

    protected void WalkWhileFaceToMC(Vector3 move)
    {
        characterController.Move(move);

        move.y = 0f;
        float smoothLambda = Mathf.Min(1.0f, Time.deltaTime / 0.1f);
        smoothMove = Vector3.Lerp(smoothMove, move, smoothLambda);

        if (move.magnitude >= 0.0001f)
        {
            if (smoothMove.magnitude > 0.005f)
            {
                Vector3 faceDir = EnemyUtility.GetMCPosition() - transform.position;
                faceDir.y = 0f;
                nowForward = faceDir.normalized;

                float f = Vector3.Dot(transform.forward, move) / (agent.speed * Time.deltaTime);
                float r = Vector3.Dot(Vector3.Cross(Vector3.up, transform.forward), move) / (agent.speed * Time.deltaTime);

                animator.SetFloat(Walk_Blend_F_Name, f);
                animator.SetFloat(Walk_Blend_R_Name, r);
            }
        }
    }

    // interfaces
    virtual public void EnterExecution()
    {
        behaviorTree.SetVariableValue("executed", true);
        behaviorTree.SetVariableValue("hurt", true);
    }

    virtual public void StepExecution()
    {
        ;
    }
}
