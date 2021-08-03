using System;
using UnityEngine;
using UnityEngine.Events;

namespace Bots
{
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed;
        [SerializeField] private float jumpVelocity = 5f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float slowFallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        [SerializeField] private LayerMask groundMask;

        [SerializeField] private bool slowFall;

        private CameraController _cameraController;
        private Rigidbody _rigidbody;

        public UnityEvent OnJumped = new UnityEvent();
        public bool jumping;
        public UnityEvent OnLanded = new UnityEvent();

        public bool OnProgressPath;
        public ProgressPath progressPath;

        private bool doJump;
        private Vector3 move;

        private void Awake()
        {
            _cameraController = FindObjectOfType<CameraController>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (IsGrounded() || OnProgressPath)
                {
                    OnProgressPath = false;
                    _rigidbody.velocity = Vector3.up * jumpVelocity;
                }
            }


            if (OnProgressPath)
            {
                ProgressPathMove();
                return;
            }

            // HorizontalMove();


            MakeJumpNice();

            if (IsGrounded() && _rigidbody.velocity.y > 0 && !jumping)
            {
                jumping = true;
                OnJumped.Invoke();
            }

            if (IsGrounded() && _rigidbody.velocity.y < 0 && jumping)
            {
                jumping = false;
                OnLanded.Invoke();
            }
        }

        public void ToggleProgressPath(bool value)
        {
            OnProgressPath = value;
            _rigidbody.useGravity = !value;
        }

        public void ToggleProgressPath(bool value, ProgressPath path)
        {
            progressPath = path;
            ToggleProgressPath(value);
        }

        private void ProgressPathMove()
        {
            var verticalAxis = Input.GetAxis("Vertical");
            progressPath.Move(verticalAxis * moveSpeed * Time.deltaTime);
        }

        private void HorizontalMove()
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            move *= moveSpeed * Time.deltaTime;
            _rigidbody.velocity = transform.rotation * new Vector3(move.x, _rigidbody.velocity.y, move.y);
        }

        public bool IsGrounded()
        {
            return Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f, groundMask);
        }

        private void MakeJumpNice()
        {
            if (_rigidbody.velocity.y < 0)
            {
                var fallMult = slowFall ? slowFallMultiplier : fallMultiplier;
                _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (fallMult - 1) * Time.deltaTime);
            }
            else if (_rigidbody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                _rigidbody.velocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime);
            }
        }

        public void ToggleSlowFall(bool value) => slowFall = value;
    }
}