using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DollyCameraSet
{
    [SerializeField]
    private CinemachineVirtualCamera camera;
    [SerializeField]
    private CinemachineDollyCart lookAt;
    private CinemachineTrackedDolly cameraDolly = null;

    public void SetActive(bool value)
    {
        camera.gameObject.SetActive(value);
        // lookAt.gameObject.SetActive(value);
    }

    public void Reset()
    {
        if (cameraDolly == null)
        {
            cameraDolly = camera.GetCinemachineComponent<CinemachineTrackedDolly>();
        }

        SetPosition(0f);
    }

    public void SetPosition(float position)
    {
        cameraDolly.m_PathPosition = position;
        lookAt.m_Position = position;
    }
}
