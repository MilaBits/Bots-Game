using Unity.Mathematics;
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

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            transform.SetParent(verticalPivot.transform);
            transform.localPosition = offset;
            transform.localRotation = quaternion.identity;
        }

        void Update()
        {
            horizontalPivot.eulerAngles += new Vector3(0, Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime, 0);
            verticalPivot.eulerAngles = verticalPivot.eulerAngles + new Vector3(Input.GetAxis("Mouse Y") * -sensitivityY * Time.deltaTime, 0, 0);

            transform.localPosition += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel"));
        }
    }
}