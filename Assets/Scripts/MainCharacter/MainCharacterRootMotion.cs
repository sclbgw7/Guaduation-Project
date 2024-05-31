using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterRootMotion : MonoBehaviour
{
    private Animator animator;
    private CharacterController parentController;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        parentController = transform.parent.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 dPosDum = Vector3.zero;
    void OnAnimatorMove()
    {
        // temporarily deprecated

        animator.ApplyBuiltinRootMotion();
        Vector3 deltaPosition = animator.deltaPosition;

        transform.position -= deltaPosition;

        // forward motion to the parent object
        parentController.Move(deltaPosition);
    }
}
