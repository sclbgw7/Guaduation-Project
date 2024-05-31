using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance => instance;
    private static CameraManager instance;

    [SerializeField]
    private FollowAndFaceAtTarget followAndFaceAtTarget;

    [SerializeField]
    private CinemachineFreeLook freeCamera;
    [SerializeField]
    private CinemachineFreeLook lockCamera;
    private CinemachineVirtualCameraBase activeCamera;

    [SerializeField]
    private DollyCameraSet reboundCameraSet;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        activeCamera = freeCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCameraObjects()
    {
        followAndFaceAtTarget.UpdateManually();
    }

    public void SwitchToLockCamera()
    {
        activeCamera.gameObject.SetActive(false);
        lockCamera.gameObject.SetActive(true);
        lockCamera.m_YAxis.Value = 0.5f;
        activeCamera = lockCamera;
    }

    public void SwitchToFreeCamera()
    {
        activeCamera.gameObject.SetActive(false);
        if (activeCamera == lockCamera)
        {
            freeCamera.ForceCameraPosition(lockCamera.transform.position, lockCamera.transform.rotation);
        }
        freeCamera.gameObject.SetActive(true);
        activeCamera = freeCamera;
        followAndFaceAtTarget.SetFaceAt(null);
    }

    public void SwitchLockingTarget(LockableTarget newTarget)
    {
        followAndFaceAtTarget.SetFaceAt(newTarget.transform);
    }

    public DollyCameraSet GetReboundCameraSet()
    {
        return reboundCameraSet;
    }
}
