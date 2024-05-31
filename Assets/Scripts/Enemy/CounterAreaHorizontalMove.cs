using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAreaHorizontalMove : CounterArea
{
    [SerializeField]
    private Transform axeBlade;

    void Start()
    {
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = axeBlade.position;
        pos.y = owner.transform.position.y;

        Vector3 angle = axeBlade.rotation.eulerAngles;
        angle.z = 0;
        angle.x = 0;

        mainCollider.transform.position = pos;
        mainCollider.transform.rotation = Quaternion.Euler(angle);
    }
}
