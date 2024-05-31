using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class MainCharacterInputManager : MonoBehaviour
{
    private MainCharacterMovement movement;
    private MainCharacterAnimParameters animParameters;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float preInputTime;

    private struct LastingCommand
    {
        public int id;
        public float lastTime;
    }

    private Dictionary<int, Func<Animator, bool>> commandActions;
    private LinkedList<LastingCommand> commandQueue;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<MainCharacterMovement>();
        animParameters = movement.animParameters;
        // should set animator in Inspector...

        commandActions = new Dictionary<int, Func<Animator, bool>>();
        commandActions[0] = (animator) => animParameters.SetTriggerJump(animator);
        commandActions[1] = (animator) => animParameters.SetTriggerFire1(animator);
        commandActions[2] = (animator) => animParameters.SetTriggerRoll(animator);

        commandQueue = new LinkedList<LastingCommand>();
    }

    // Update is called once per frame
    void Update()
    {
        // We're now disabling jumping.
        // Maybe temporally.
        /*
        if (Input.GetButtonDown("Jump"))
        {
            InsertPreInput(0);
        }
        */
        if (Input.GetButtonDown("Fire1"))
        {
            InsertPreInput(1);
        }
        if (Input.GetButtonDown("Roll"))
        {
            InsertPreInput(2);
        }

        var node = commandQueue.First;
        while (node != null)
        {
            if(Time.unscaledTime - node.Value.lastTime <= preInputTime)
            {
                //Debug.Log(Time.unscaledTime - node.Value.lastTime);
                bool succeed = commandActions[node.Value.id](animator);
                if (succeed)
                {
                    commandQueue.Remove(node);
                    break;
                }
            }
            else
            {
                commandQueue.Remove(node);
            }
            node = node.Next;
        }
    }

    private void InsertPreInput(int id)
    {
        var node = commandQueue.First;
        while (node != null)
        {
            if(node.Value.id == id)
            {
                commandQueue.Remove(node);
            }
            node = node.Next;
        }
        commandQueue.AddFirst(new LastingCommand { id = id, lastTime =  Time.unscaledTime });
    }
}
