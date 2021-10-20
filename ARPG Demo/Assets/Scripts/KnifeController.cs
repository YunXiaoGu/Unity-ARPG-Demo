using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    // 第三人称控制器
    public class KnifeController : MonoBehaviour
    {
        private float hInput; // X轴输入
        private float vInput; // Y轴输入
        private Vector2 input; // X Y轴输入组成的Vector2
        private float turnSmooth = 0.2f; // 转身平滑
        private float weight = 0f; // 动画权重
        private new Rigidbody rigidbody;
        private Animator animator;       // 动画控制器
        private Transform cameraTransform; // 相机

        private float targetRotation; // 角色每帧的旋转角度
        private float currentVelocity;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;
        }

        void Update()
        {

            currentAnimStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            hInput = Input.GetAxis("Horizontal");
            vInput = Input.GetAxis("Vertical");
            input = new Vector2(hInput, vInput);
            if (vInput != 0 || hInput != 0)
            {
                // 角色动起来
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    // 角色转向角度 = 方向键产生的角度 + 摄像机旋转产生的角度
                    targetRotation = Mathf.Atan2(input.normalized.x, input.normalized.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                    // 角色旋转
                    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref currentVelocity, turnSmooth);

                    // 走路
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        animator.SetBool("IsWalk", true);
                        animator.SetBool("IsRun", false);
                    }
                    // 奔跑
                    else
                    {
                        animator.SetBool("IsRun", true);
                        animator.SetBool("IsWalk", false);
                    }
                    // 跳跃
                    if (Input.GetKey(KeyCode.Space))
                    {
                        animator.SetBool("IsJump", true);
                    }
                }
            }
            else
            {
                animator.SetBool("IsWalk", false);
                animator.SetBool("IsRun", false);

            }
            // 跳跃（原地）
            if (Input.GetKey(KeyCode.Space) && !isJump)
            {
                isJump = true;
                animator.SetBool("IsJump", isJump);
            }
            Attack();

            // 翻滚
            if (Input.GetKey(KeyCode.R))
            {
                animator.SetTrigger("RollTrigger");
            }
        }

        private AnimatorStateInfo currentAnimStateInfo;
        private bool isAttack = false;
        private int attackValue = 0;
        private float jumpForce = 9000f;
        private bool isJump = false;
        void FixedUpdate()
        {
            // 角色在移动状态时：攻击中是不能移动，移动交给动画自带位移；
            if (animator.GetBool("IsWalk") && animator.GetInteger("AttackValue") == 0)
            {
                // 角色移动
                rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * 2f);
            }
            else if (animator.GetBool("IsRun") && animator.GetInteger("AttackValue") == 0)
            {
                // 角色移动
                rigidbody.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * 6f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            isJump = false;
            animator.SetBool("IsJump", isJump);
        }

        // 当角色处于Idle状态时，该方法在 原地跳跃动画[三段动画]的起跳阶段完成时，在末尾动画帧事件调用，给角色施加向上的力，达到跳跃效果；
        // 注：此时的动画不带有位移效果，所以通过施加力来实现位移。
        private void JumpUpInPlace()
        {
            isJump = true;
            rigidbody.AddForce(Vector3.up * jumpForce);
        }

        // 当角色在运动时，该方法在该方法在 移动跳跃动画[三段动画]的起跳阶段完成时，在末尾动画帧事件调用，给角色施加向上和向前的力，达到角色跳跃时移动效果
        private void JumpWhileMoving()
        {
            rigidbody.AddForce(Vector3.up * jumpForce * 1.5f);
        }

        private void Attack()
        {
            //若动画为三种状态之一并且已经播放完毕
            if ((currentAnimStateInfo.IsName("R_Attack_05") || currentAnimStateInfo.IsName("R_Attack_09") || currentAnimStateInfo.IsName("R_Attack_12")) && currentAnimStateInfo.normalizedTime > 1f)
            {
                attackValue = 0;   //将 attackValue 重置为0，即 Idle 状态
                animator.SetInteger("AttackValue", attackValue);
            }
            //按下鼠标左键键攻击
            if (Input.GetMouseButtonDown(0))
            {
                //若处于Idle状态，则直接打断并过渡到 R_Attack_05 (攻击阶段一)
                if ((currentAnimStateInfo.IsName("Idle") || currentAnimStateInfo.IsName("Walk") || currentAnimStateInfo.IsName("Run")) && attackValue == 0)
                {
                    attackValue = 1;
                    animator.SetInteger("AttackValue", attackValue);
                }
                //如果当前动画处于 R_Attack_05 (攻击阶段一)并且该动画播放进度小于80%，此时按下攻击键可过渡到攻击阶段二
                else if (currentAnimStateInfo.IsName("R_Attack_05") && attackValue == 1 && currentAnimStateInfo.normalizedTime < 0.6f)
                {
                    attackValue = 2;
                    animator.SetInteger("AttackValue", attackValue);
                }
                // //如果当前动画处于 R_Attack_09 (攻击阶段二)并且该动画播放进度小于80%，此时按下攻击键可过渡到攻击阶段二
                else if (currentAnimStateInfo.IsName("R_Attack_09") && attackValue == 2 && currentAnimStateInfo.normalizedTime < 0.8f)
                {
                    attackValue = 3;
                    animator.SetInteger("AttackValue", attackValue);
                }
            }
        }

        private void SwitchNextAttackAnimation()
        {
            animator.SetInteger("AttackValue", attackValue);
        }
    }
}
