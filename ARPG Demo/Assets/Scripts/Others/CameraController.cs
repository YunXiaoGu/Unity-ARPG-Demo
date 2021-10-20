using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 摄像机控制器
public class CameraController : MonoBehaviour
{
    public Transform target;
    public float xSpeed = 200f;
    public float ySpeed = 200f;

    public float mSpeed = 10f;
    public float yMinLimit = -50f;
    public float yMaxLimit = 50f;
    public float distance = 10f;
    public float minDistance = 2f;
    public float maxDistance = 30f;


    public bool needDamping = true;
    private float damping = 5f;
    private float x = 0f;
    private float y = 0f;

    void Start()
    {
        Vector3 angle = transform.eulerAngles;
        x = angle.y;
        y = angle.y;
    }

    void LateUpdate()
    {
        if (target)
        {
            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            Quaternion rotation = Quaternion.Euler(y, x, 0f);

            distance -= Input.GetAxis("Mouse ScrollWheel") * mSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            Vector3 distanceVector = new Vector3(0f, 0f, -distance);
            
            Vector3 position = rotation * distanceVector + target.position;
            // 调整摄像机
            if (needDamping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * damping);
            }
            else
            {
                transform.rotation = rotation;
                transform.position = position;
            }
        }
    }

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
