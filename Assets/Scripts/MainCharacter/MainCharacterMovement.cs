using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCharacterMovement : MonoBehaviour
{
    public LayerMask groundLayer;

    public MainCharacterAnimParameters animParameters;

    // will be priavte ---
    public float moveSpeed = 2.0f;

    public float jumpHeight = 1.0f;
    public float gravityValue = -9.81f;

    private float turnVelo; // calculated in Start()
    public float turnRatio = 10.0f;
    public float linearThreshlod = 30.0f;

    public float groundDetectDis = 0.1f;
    public float fallingHeight = 0.5f;
    public float closestDisFormEnemy;
    // end --------------------

    private CharacterController controller;
    [SerializeField]
    private Animator animator;
    private LockedTargetManager lockedTargetManager;
    private Vector3 playerVelocity;
    // private bool groundedPlayer;

    private Vector3 nowForward;
    private Vector3 moveDirection; // for anim motion
    private float nowMoveX;
    private float nowMoveZ;
    private bool stopMovingInThisAttack;

    void Awake()
    {
        animParameters = new MainCharacterAnimParameters();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        // animator = GetComponent<Animator>();
        lockedTargetManager = LockedTargetManager.Instance;

        turnVelo = turnRatio * linearThreshlod;
        nowForward = transform.forward;
        nowMoveX = 0f;
        nowMoveZ = 0f;
        stopMovingInThisAttack = false;

        animParameters.applyStateMovement += this.ApplyStateMachineMovement;
        animParameters.singleAttackOut += this.ClearAttackMovingState;
    }

    // Update is called once per frame
    void Update()
    {
        animParameters.isGround = GetGrounded();
        animParameters.shouldFall = GetShouldFall();
        if (animParameters.isGround && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (animParameters.canMove && move != Vector3.zero)
        {
            UpdateForward(move);

            if (lockedTargetManager.lockingMode)
            {
                move.Normalize();
                TweenRunDirection(move);
                ControllerApplyMove(
                        (
                            nowForward * move.z + 
                            Vector3.Cross(Vector3.up, nowForward) * move.x
                        ) * Time.deltaTime * moveSpeed
                    );

                // Debug.Log((nowForward * move.z + Vector3.Cross(Vector3.up, nowForward) * move.x).magnitude);
            }
            else
            {
                ResetRunDirection();
                animator.SetFloat("Run_Blend_R", 0f);
                animator.SetFloat("Run_Blend_F", 1f);
                ControllerApplyMove(nowForward * Time.deltaTime * moveSpeed);
            }
        }
        animParameters.speed = move == Vector3.zero ? 0f : 1f;
        RotateModelToForward();

        // Changes the height position of the player..
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void RotateModelToForward()
    {
        float angleGap = Vector3.Angle(gameObject.transform.forward, nowForward); // * 180.0f / Mathf.PI;
        //Debug.Log(angleGap);
        float tLerp = angleGap > linearThreshlod ? 
            turnRatio * Time.deltaTime :
            turnVelo * Time.deltaTime / angleGap;
        if(tLerp >= 1)
        {
            gameObject.transform.forward = nowForward;
        }
        else
        {
            gameObject.transform.forward =
                Quaternion.Lerp(
                        Quaternion.LookRotation(gameObject.transform.forward),
                        Quaternion.LookRotation(nowForward),
                        tLerp
                    ) * Vector3.forward;
        }
        
    }

    private void TweenRunDirection(Vector3 move)
    {
        // transit in 0.2s
        MathUtility.SimpleTweenFloat(ref nowMoveX, move.x, 0.2f);
        animator.SetFloat("Run_Blend_R", nowMoveX);

        MathUtility.SimpleTweenFloat(ref nowMoveZ, move.z, 0.2f);
        animator.SetFloat("Run_Blend_F", nowMoveZ);
    }

    private void ResetRunDirection()
    {
        nowMoveX = 0;
        nowMoveZ = 0;
    }

    private bool GetGrounded()
    {
        return Physics.OverlapSphere(transform.position + Vector3.up * (0.5f - groundDetectDis), 0.5f, groundLayer).Length > 0;
        // return Physics.Raycast(transform.position + Vector3.up * groundDetectDis * 0.5f, Vector3.down, groundDetectDis, groundLayer);
        
        // return controller.isGrounded;
    }

    private bool GetShouldFall(Vector3 deltaX = new Vector3())
    {
        Vector3 point0 = transform.position + deltaX + Vector3.up * (0.5f);
        Vector3 point1 = point0 + Vector3.down * fallingHeight;
        return Physics.OverlapCapsule(point0, point1, 0.5f, groundLayer).Length == 0;
    }

    private void ControllerApplyMove(Vector3 move)
    {
        bool blocked = NavMesh.Raycast(transform.position, transform.position + move, out NavMeshHit hit, NavMesh.AllAreas);

        // Debug.DrawLine(hit.position, hit.position + hit.normal, Color.red);

        if (!animParameters.isJumping && blocked && Vector3.Dot(move, hit.normal) < 0f)
        {
            Vector3 localMoveDir = Vector3.Cross(Vector3.up, hit.normal);
            move = localMoveDir * Vector3.Dot(localMoveDir, move);
            
            blocked = NavMesh.Raycast(transform.position, transform.position + move, out hit, NavMesh.AllAreas);
            if (!blocked)
            {
                move.y = -move.magnitude;
                controller.Move(move);
            }
        }
        else
        {
            if(!animParameters.isJumping)
            {
                move.y = -move.magnitude;
            }
            controller.Move(move);
        }
    }

    private void Jump()
    {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
    }

    private void UpdateForward(Vector3 move)
    {
        if (lockedTargetManager.lockingMode)
        {
            Vector3 dis = lockedTargetManager.lockingPosition - transform.position;
            dis.y = 0f;
            nowForward = dis.normalized;
        }
        else if (move != Vector3.zero)
        {
            Vector3 forwardCam = Camera.main.transform.forward;
            forwardCam.y = 0f;
            forwardCam.Normalize();
            Vector3 rightCam = Vector3.Cross(Vector3.up, forwardCam);

            move.Normalize();
            move = forwardCam * move.z + rightCam * move.x;
            nowForward = move;
        }
        moveDirection = nowForward;
    }

    private void RollInLockingMode(Vector3 move)
    {
        if (move == Vector3.zero)
        {
            move.z = 1f;
        }
        move.Normalize();
        moveDirection = nowForward * move.z + Vector3.Cross(Vector3.up, nowForward) * move.x;

        float angle = Vector3.Angle(nowForward, moveDirection);
        angle *= Mathf.Sign(Vector3.Cross(nowForward, moveDirection).y);
        //Debug.Log(angle);
        
        if (angle > -110f && angle < 110f)
        {
            animator.SetFloat("Roll_Blend", angle / 90f);
        }
        else
        {
            animator.SetFloat("Roll_Blend", 0f);
            nowForward = moveDirection;
            transform.forward = nowForward;
        }
        
    }

    private void ApplyStateMachineMovement()
    {
        // read "animator to character" parameters
        if (animParameters.shouldJump)
        {
            Jump();
            animParameters.shouldJump = false;
        }
        if (animParameters.shouldRoll)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            UpdateForward(move);
            if (lockedTargetManager.lockingMode)
            {
                RollInLockingMode(move);
            }
            else
            {
                animator.SetFloat("Roll_Blend", 0f);
            }
            animParameters.shouldRoll = false;
        }
        if (animParameters.shouldUpdateForward)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            UpdateForward(move);
            animParameters.shouldUpdateForward = false;
        }
        if (animParameters.forwardMoveDeltaDis != 0f)
        {
            if (lockedTargetManager.lockingMode)
            {
                animParameters.forwardMoveDeltaDis *= animParameters.moveScaleWhenLocking;
            }

            // Only when attacking a locking target:
            // stop forward movement if it's close enough to the target.

            // What's more, once you reach the target's position in an attack,
            // you shouldn't move anymore during the remaining time of the attack,
            // especially in case that an enemy is moving backwards.
            // Otherwise, the position of your character may fliker.

            float disFormTarget;
            if (animParameters.isAttacking && lockedTargetManager.lockingMode)
            {
                if (IsCloseToTarget(out disFormTarget))
                {
                    stopMovingInThisAttack = true;
                }
            }
            if (!stopMovingInThisAttack)
            {
                ControllerApplyMove(moveDirection * animParameters.forwardMoveDeltaDis);
            }
            
            animParameters.forwardMoveDeltaDis = 0f;
        }

        // Camera-relating logics should be calculated after the state machine is updated.
        CameraManager.Instance.UpdateCameraObjects();
    }

    private bool IsCloseToTarget(out float disFormTarget)
    {
        lockedTargetManager.nowLocking.mainCollider.Raycast(
            new Ray(transform.position, lockedTargetManager.lockingPosition - transform.position), 
            out RaycastHit hitInfo, 
            Mathf.Infinity);
        Vector3 dis = hitInfo.point - transform.position;
        dis.y = 0f;

        
        disFormTarget = dis.magnitude;
        return disFormTarget < closestDisFormEnemy;
    }

    private void ClearAttackMovingState()
    {
        stopMovingInThisAttack = false;
    }

    // Public methods
    public void UpdateForwardWhenHit(Vector3 hitDir)
    {
        hitDir.y = 0f;
        hitDir.Normalize();

        nowForward = -hitDir;
        moveDirection = nowForward;
    }

    public void UpdateForwardTowards(Vector3 toward)
    {
        Vector3 dir = toward - transform.position;
        dir.y = 0f;
        dir.Normalize();

        nowForward = dir;
        moveDirection = nowForward;
    }
}
