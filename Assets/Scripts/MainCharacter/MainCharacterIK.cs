using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainCharacterIK : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnAnimatorIK(int layerIndex)
    {
        Vector3 leftFootPos = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 targetPosLF = Vector3.zero;
        if (Physics.Raycast(leftFootPos + Vector3.up, Vector3.down, out RaycastHit hitInfoLF, Mathf.Infinity, groundLayer))
        {
            targetPosLF = hitInfoLF.point;
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, targetPosLF);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            /*
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, rotation);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            */
        }

        Vector3 rightFootPos = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 targetPosRF = Vector3.zero;
        if (Physics.Raycast(rightFootPos + Vector3.up, Vector3.down, out RaycastHit hitInfoRF, Mathf.Infinity, groundLayer))
        {
            targetPosRF = hitInfoRF.point;
            animator.SetIKPosition(AvatarIKGoal.RightFoot, targetPosRF);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
            /*
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rotation);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
            */
        }
        
    }
}
