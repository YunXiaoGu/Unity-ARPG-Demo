using UnityEngine;


namespace Script.Others
{
    // 第三人称控制器
    public class ThirdPlayerController : MonoBehaviour
    {
        private float h;
        private float v;
        public float speed = 6f;
        public float turnSpeed = 15f;
        public Transform cameraTransform;
        private Vector3 movement;
        private Vector3 cameraForward;

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            transform.Translate(cameraTransform.right * h * speed * Time.deltaTime + cameraForward * v * speed * Time.deltaTime, Space.World);
            if (h != 0 || v != 0)
            {
                Rotating(h, v);
            }
        }

        // 转身
        private void Rotating(float h, float v)
        {
            cameraForward = Vector3.Cross(cameraTransform.right, Vector3.up);
            Vector3 targetDirection = cameraTransform.right * h + cameraForward * v;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }
}
