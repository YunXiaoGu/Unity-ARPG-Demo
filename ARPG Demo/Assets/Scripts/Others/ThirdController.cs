using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdController : MonoBehaviour
{
    private float hInput; // X轴输入
    private float vInput; // Y轴输入
    private new Rigidbody rigidbody;
    private Animator animator;       // 动画控制器
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        if (vInput != 0)
        {
            animator.SetFloat("Speed", vInput);
            Debug.Log(vInput);
        }
    }
}
