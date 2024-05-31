using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LockedTargetManager : MonoBehaviour
{
    public static LockedTargetManager Instance => instance;
    private static LockedTargetManager instance;

    public bool lockingMode { get; private set; }
    public Vector3 lockingPosition { get => nowLocking.GetLockingPosition(); }
    public LockableTarget nowLocking { get; private set; }

    [SerializeField]
    private Transform mainCharacterTransform;
    [SerializeField]
    private float slidingMouseXThreshold;
    [SerializeField]
    private float slidingTimeThreshold;
    [SerializeField]
    private GameObject lockingDotUI;

    private LinkedList<LockableTarget> targetList;

    private float mouseSlidingTime;
    private float filteredXInput;
    private bool mouseSlidingDirRight;

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

        targetList = new LinkedList<LockableTarget>();
    }

    void Start()
    {
        lockingMode = false;
        lockingDotUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Lock"))
        {
            SwitchLockingMode();
        }

        // TODO: After packaging, The deviceXInput may suddenly turn to 0
        // during a continuous move, which is really confusing.
        // So it must be filtered.

        float deviceXInput = Input.GetAxis("Mouse X") + Input.GetAxis("Joystick RS X");
        
        float smoothLambda = Mathf.Min(1.0f, Time.deltaTime / 0.03f);
        filteredXInput = Mathf.Lerp(filteredXInput, deviceXInput, smoothLambda);

        // Debug.Log(deviceXInput);

        if (filteredXInput > slidingMouseXThreshold)
        {
            if (!mouseSlidingDirRight)
            {
                mouseSlidingTime = 0f;
            }
            mouseSlidingDirRight = true;
            mouseSlidingTime += Time.unscaledDeltaTime;
        }
        else if (filteredXInput < -slidingMouseXThreshold)
        {
            if (mouseSlidingDirRight)
            {
                mouseSlidingTime = 0f;
            }
            mouseSlidingDirRight = false;
            mouseSlidingTime += Time.unscaledDeltaTime;
        }
        else
        {
            mouseSlidingTime = 0f;
        }

        if (mouseSlidingTime >= slidingTimeThreshold)
        {
            if (lockingMode)
            {
                // Debug.Log("switching");
                SwitchToNextTarget(mouseSlidingDirRight);
            }
            mouseSlidingTime = 0f;
        }

        if (lockingMode && targetList.Count == 0)
        {
            SwitchLockingMode();
        }

        
    }

    void LateUpdate()
    {
        // display the locking dot UI
        if (lockingMode)
        {
            Camera mainCam = Camera.main;
            Vector2 screenPoint = mainCam.WorldToScreenPoint(lockingPosition);

            RectTransform rect = lockingDotUI.transform as RectTransform;
            RectTransform parentRect = rect.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out Vector2 uiPoint);

            // Debug.Log(uiPoint);

            rect.anchoredPosition = uiPoint;
        }
    }

    public void AddTarget(LockableTarget target)
    {
        targetList.AddFirst(target);
        // Debug.Log($"Add Target! Count: {targetList.Count}");
    }

    public void RemoveTarget(LockableTarget target)
    {
        targetList.Remove(target);
        // Debug.Log($"Remove Target! Count: {targetList.Count}");
    }

    private void SwitchLockingMode()
    {
        if (!lockingMode)
        {
            if (targetList.Count > 0)
            {
                LockableTarget recordedTarget = null;
                float recordedAngle = 180f;

                Vector3 r1 = Camera.main.transform.forward;

                foreach (LockableTarget target in targetList)
                {
                    float newAngle = CalcAngle(r1, target.GetLockingPosition() - mainCharacterTransform.position);
                    // Debug.Log(newAngle);

                    if (Mathf.Abs(newAngle) < recordedAngle)
                    {
                        recordedAngle = Mathf.Abs(newAngle);
                        recordedTarget = target;
                    }
                }

                nowLocking = recordedTarget;
                if (nowLocking == null)
                {
                    Debug.LogError("Locking target not found correctly!");
                    nowLocking = targetList.First.Value;
                }

                lockingMode = true;
                CameraManager.Instance.SwitchToLockCamera();
                CameraManager.Instance.SwitchLockingTarget(nowLocking);
                lockingDotUI.SetActive(true);
            }
        }
        else
        {
            nowLocking = null;
            lockingMode = false;
            CameraManager.Instance.SwitchToFreeCamera();
            lockingDotUI.SetActive(false);
        }
    }

    private void SwitchToNextTarget(bool toRight)
    {
        LockableTarget recordedTarget = nowLocking;
        float recordedAngle = toRight ? 180f : -180f;

        Vector3 r1 = nowLocking.GetLockingPosition() - mainCharacterTransform.position;

        foreach (LockableTarget target in targetList)
        {
            if (target == nowLocking)
            {
                continue;
            }
            float newAngle = CalcAngle(r1, target.GetLockingPosition() - mainCharacterTransform.position);
            // Debug.Log(newAngle);

            if (toRight)
            {
                if(newAngle < recordedAngle && newAngle > 0f)
                {
                    recordedAngle = newAngle;
                    recordedTarget = target;
                }
            }
            else // ro left
            {
                if (newAngle > recordedAngle && newAngle < 0f)
                {
                    recordedAngle = newAngle;
                    recordedTarget = target;
                }
            }
        }

        nowLocking = recordedTarget;
        CameraManager.Instance.SwitchLockingTarget(nowLocking);
    }

    private float CalcAngle(Vector3 r1, Vector3 r2)
    {
        r1.y = 0f;
        r2.y = 0f;

        float angle = Vector3.Angle(r1, r2);
        angle *= Mathf.Sign(Vector3.Cross(r1, r2).y);

        return angle;
    }
}
