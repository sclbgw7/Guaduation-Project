using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class EnemyUtility : MonoBehaviour
{
    public static EnemyUtility Instance => instance;
    private static EnemyUtility instance;

    [SerializeField]
    private MainCharacterBattle _mainCharacter;
    public MainCharacterBattle mainCharacter { get => _mainCharacter; }
    [SerializeField]
    private LayerMask playerLayer;

    private Dictionary<EnemyAttackTask, List<Coroutine>> taskAndCoroutineTable;
    private Dictionary<EnemyAttackTask, List<CounterArea>> taskAndCounterAreaTable;
    private NavMeshPath pathCache;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }

        pathCache = new NavMeshPath();
        taskAndCoroutineTable = new Dictionary<EnemyAttackTask, List<Coroutine>>();
        taskAndCounterAreaTable = new Dictionary<EnemyAttackTask, List<CounterArea>>();
    }

    public static Vector3 GetMCPosition()
    {
        return Instance.mainCharacter.transform.position;
    }

    public static float CalcDisToMCOnNavMesh(Vector3 enemyPosition)
    {
        NavMesh.CalculatePath(enemyPosition, GetMCPosition(), NavMesh.AllAreas, Instance.pathCache);

        if (Instance.pathCache.status == NavMeshPathStatus.PathInvalid ||
            Instance.pathCache.corners.Length == 0)
            return -1f;

        float distance = 0.0f;
        for (int i = 0; i < Instance.pathCache.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(Instance.pathCache.corners[i], Instance.pathCache.corners[i + 1]);
        }

        return distance;
    }

    public static void StartAttackTask(EnemyAttackTask task, Enemy launcher) => Instance._StartAttackTask(task, launcher);
    private void _StartAttackTask(EnemyAttackTask task, Enemy launcher)
    {
        taskAndCoroutineTable.Add(task, new List<Coroutine>());
        foreach (EnemyAttackParam attackEvent in task.normalAttackEvents)
        {
            taskAndCoroutineTable[task].Add(StartCoroutine(AttackEventCoroutine(attackEvent, launcher)));
        }

        if (task.counterableAttackEvents.Count > 0)
        {
            taskAndCounterAreaTable.Add(task, new List<CounterArea>());
            foreach (EnemyCounterableAttackEvent attackEvent in task.counterableAttackEvents)
            {
                taskAndCoroutineTable[task].Add(StartCoroutine(AttackEventCoroutine(attackEvent.normalParameters, launcher)));
                taskAndCoroutineTable[task].Add(StartCoroutine(CounterableAttackEventCoroutine(attackEvent.counterParameters, launcher)));
                taskAndCounterAreaTable[task].Add(attackEvent.counterParameters.counterArea);
            }
        }
    }

    public static void StopAttackTask(EnemyAttackTask task) => Instance._StopAttackTask(task);
    private void _StopAttackTask(EnemyAttackTask task)
    {
        foreach (Coroutine item in taskAndCoroutineTable[task])
        {
            StopCoroutine(item);
        }
        taskAndCoroutineTable.Remove(task);

        if (taskAndCounterAreaTable.ContainsKey(task))
        {
            foreach (CounterArea counterArea in taskAndCounterAreaTable[task])
            {
                // TODO: have not considered if the enemy has been destroyed
                counterArea.gameObject.SetActive(false);
                counterArea.GetOwner().counterAreaActive = false;
            }
            taskAndCounterAreaTable.Remove(task);
        }
    }

    
    private IEnumerator AttackEventCoroutine(EnemyAttackParam attackEvent, Enemy launcher)
    {
        float waitTime = attackEvent.attackFrameL / 30.0f;
        float endTime = attackEvent.attackFrameR / 30.0f;
        float beginTime = Time.time;

        yield return new WaitForSeconds(waitTime);

        Vector3 shakeDir = (attackEvent.weaponArea.transform.rotation * attackEvent.localShakeDirection) * attackEvent.shakeForce;

        Vector3 lastWorldCenter = attackEvent.weaponArea.transform.TransformPoint(attackEvent.weaponArea.center);

        MainCharacterBattle hitTarget = null;
        while (Time.time - beginTime <= endTime)
        {
            if (hitTarget == null)
            {
                Vector3 worldCenter = attackEvent.weaponArea.transform.TransformPoint(attackEvent.weaponArea.center);
                Vector3 worldHalfExtents = Vector3.Scale(attackEvent.weaponArea.size, attackEvent.weaponArea.transform.lossyScale) * 0.5f;

                Vector3 castDir = lastWorldCenter - worldCenter;

                if (castDir.magnitude < 0.01f)
                {
                    Collider[] hitColliders = Physics.OverlapBox(
                        worldCenter,
                        worldHalfExtents,
                        attackEvent.weaponArea.transform.rotation,
                        playerLayer);

                    if (hitColliders.Length >= 2)
                    {
                        throw new System.InvalidOperationException("OverlapBox found more than 1 players, which is impossible.");
                    }
                    if (hitColliders.Length > 0)
                    {
                        hitTarget = hitColliders[0].GetComponent<MainCharacterBattle>();
                    }
                }
                else
                {
                    RaycastHit[] hits = Physics.BoxCastAll(
                        worldCenter,
                        worldHalfExtents,
                        castDir.normalized,
                        attackEvent.weaponArea.transform.rotation,
                        castDir.magnitude,
                        playerLayer);
                    // Debug.Log($"{worldCenter}  {worldHalfExtents} {attackEvent.weaponArea.transform.rotation}");

                    if (hits.Length >= 2)
                    {
                        throw new System.InvalidOperationException("BoxCastAll found more than 1 players, which is impossible.");
                    }
                    if (hits.Length > 0)
                    {
                        hitTarget = hits[0].collider.GetComponent<MainCharacterBattle>();
                    }
                }
                lastWorldCenter = worldCenter;
            }

            if (hitTarget != null)
            {
                if (hitTarget.BladeIsGleaming() && launcher.counterAreaActive)
                {
                    ;
                }

                else
                {
                    hitTarget.Hurt(
                    new EnemyAttackEvent
                    {
                        damage = attackEvent.damage,
                        breakForce = attackEvent.breakForce
                    },
                    shakeDir,
                    launcher);
                    Debug.Log($"Enemy Hit Player! {Time.time}");

                    yield break;
                }
            }

            yield return null;
        }
        // EditorApplication.isPaused = true;
    }

    private IEnumerator CounterableAttackEventCoroutine(EnemyCounterParam counterEvent, Enemy launcher)
    {
        yield return null;

        float waitTime = counterEvent.counterFrameL / 30.0f;
        float endTime = counterEvent.counterFrameR / 30.0f;
        float beginTime = Time.time;

        yield return new WaitForSeconds(waitTime);

        counterEvent.counterArea.gameObject.SetActive(true);
        launcher.counterAreaActive = true;
        // Debug.Log("Counter area active!");

        yield return new WaitForSeconds(beginTime + endTime - Time.time);

        counterEvent.counterArea.gameObject.SetActive(false);
        launcher.counterAreaActive = false;
        // Debug.Log("Counter area inactive!");

        // If this coroutine is stopped in the midway,
        // the counter area gameObject won't be set to inactive.
        // So we have StopAttackTask to ensure this.
    }
}

public static class ExtensionMethods
{
    public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
    {
        if (navMeshAgent.pathPending ||
            navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            navMeshAgent.path.corners.Length == 0)
            return -1f;

        if (navMeshAgent.remainingDistance <= 100000.0f)
        {
            return navMeshAgent.remainingDistance;
        }

        float distance = 0.0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }

        return distance;
    }
}