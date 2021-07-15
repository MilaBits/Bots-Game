using UnityEngine;

namespace Bots
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;

        private CameraController _cameraController;

        private void Start()
        {
            _cameraController = FindObjectOfType<CameraController>();
        }

        void Update()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            move *= moveSpeed * Time.deltaTime;
            transform.Translate(new Vector3(move.x, 0, move.y));
        }
    }
}