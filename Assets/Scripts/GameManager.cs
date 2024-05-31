using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    public bool displayLines;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Application.targetFrameRate = 90;

        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnDrawGizmos()
    {
        if(displayLines)
        {
            Gizmos.color = Color.green;
            for (int i = -25; i < 25; ++i)
            {
                Gizmos.DrawLine(new Vector3(-25, 0, i), new Vector3(25, 0, i));
            }
        }
    }
}
