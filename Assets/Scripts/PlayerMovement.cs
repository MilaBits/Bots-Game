using UnityEngine;

namespace Bots
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpVelocity = 5f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        private CameraController _cameraController;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _cameraController = FindObjectOfType<CameraController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            move *= moveSpeed * Time.deltaTime;
            transform.Translate(new Vector3(move.x, 0, move.y));

            if (Input.GetKeyDown(KeyCode.Space)) _rigidbody.velocity = Vector3.up * jumpVelocity;

            MakeJumpNice();
        }

        private void MakeJumpNice()
        {
            if (_rigidbody.velocity.y < 0)
                _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            else if (_rigidbody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
                _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
        }
    }
}