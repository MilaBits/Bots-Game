using UnityEngine;

namespace Bots
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public Vector3 offset;
        public float sensitivityX;
        public float sensitivityY;
        public Transform horizontalPivot;
        public Transform verticalPivot;

        public float maxY = 60;
        public float minY = 20;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            transform.SetParent(verticalPivot.transform);
            transform.localPosition = offset;
            transform.localRotation = Quaternion.identity;
        }

        void Update()
        {
            horizontalPivot.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime, 0);
            var newX = Mathf.Clamp(verticalPivot.eulerAngles.x + Input.GetAxis("Mouse Y") * -sensitivityY * Time.deltaTime, minY, maxY);
            verticalPivot.eulerAngles = new Vector3(newX, verticalPivot.eulerAngles.y, verticalPivot.eulerAngles.z);

            transform.localPosition += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}