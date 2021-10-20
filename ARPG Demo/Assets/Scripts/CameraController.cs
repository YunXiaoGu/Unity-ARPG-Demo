using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    // 第三人称摄像机控制器
    public class CameraController : MonoBehaviour
    {
        private float yaw; // X轴数值（target是球心，摄像机纬度方向移动）
        private float pitch;// Y轴数值（target是球心，摄像机经度方向移动）
        public float yawMoveSpeed = 3f; // 摄像机水平环绕速度
        public float pitchMoveSpeed = 3f; // 摄像机垂直环绕速度
        // public Vector2 yawAngleRange = new Vector2(-45f, 45f); // 水平环绕角度
        public Vector2 pitchAngleRange = new Vector2(-45f, 90f); //  垂直环绕角度
        public Transform target; // 跟随目标
        public float damping = 5f; // 环绕阻尼
        private Vector3 defaultOffset = new Vector3(0.0f, -0.5f, 2.3f); // 默认的摄像机与角色的偏移量
        private Vector3 distance = -new Vector3(0, 0, 3f);
        void Start()
        {
            // transform.position = target.position - defaultOffset;
            Vector3 originalAngle = transform.eulerAngles;
            yaw = originalAngle.y;
            pitch = originalAngle.x;
        }

        void LateUpdate()
        {
            if (target)
            {
                yaw += Input.GetAxis("Mouse X") * yawMoveSpeed;
                pitch -= Input.GetAxis("Mouse Y") * pitchMoveSpeed;
                // 限制摄像机环绕角度
                // yaw = Mathf.Clamp(yaw, yawAngleRange.x, yawAngleRange.y);
                pitch = Mathf.Clamp(pitch, pitchAngleRange.x, pitchAngleRange.y);

                if (yaw != 0f || pitch != 0f)
                {
                    Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
                    // 摄像机旋转
                    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * damping);
                    transform.eulerAngles = new Vector3(pitch, yaw, 0f);
                    // 摄像机位移
                    // 此处用法暂没搞懂，但是在角色采用rigidbody.MovePosition()方式移动时，会产生人物剧烈抖动，应该是相机位移问题产生的人物抖动
                    // transform.position = Vector3.Lerp(transform.position, targetRotation * distance + target.position, Time.deltaTime * damping);
                    // (另一种实现) 3f 代表摄像机距离目标的距离，这种方式不会产生居角色抖动
                    transform.position = target.position - transform.forward * 3f;
                }
            }
        }

        // 限制摄像机绕目标的角度，不能直接
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
            {
                angle += 360;
            }
            if (angle > 360)
            {
                angle -= 360;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}
