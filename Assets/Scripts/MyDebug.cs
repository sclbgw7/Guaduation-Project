using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebug : MonoBehaviour
{
    public static MyDebug Instance => instance;
    private static MyDebug instance;

    private List<Vector3> displayedPositions;

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

        displayedPositions = new List<Vector3>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (displayedPositions ==  null || displayedPositions.Count == 0)
        {
            return;
        }

        Gizmos.color = new Color(1, 0, 0, 0.5f);
        foreach (var position in displayedPositions)
        {
            // Debug.Log(position);
            Gizmos.DrawCube(position, new Vector3(0.5f, 0.5f, 0.5f));
        }
        
    }

    public static void DisplayPosition(Vector3 position)
    {
        Instance.displayedPositions.Add(position);
    }
}
