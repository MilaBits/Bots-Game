using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Bots
{
    [RequireComponent(typeof(Rigidbody))]
    public class ThirdPersonMovement : MonoBehaviour
    {
        [SerializeField, Header("Movement")] public float speed = 6;
        [SerializeField] private float StopDampening = .5f;
        public float turnSmoothTime = 0.1f;
        private float turnSmoothvelocity;
        [SerializeField] private LayerMask groundMask;

        [SerializeField, Header("Jumping")] private float gravity = -9.81f;
        [SerializeField] private float jumpVelocity = 5f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float slowFallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;

        [HideInInspector] public bool slowFall;
        [HideInInspector] public bool jumping;
        [HideInInspector] public UnityEvent OnJumped = new UnityEvent();
        [HideInInspector] public UnityEvent OnLanded = new UnityEvent();

        [HideInInspector] public bool OnProgressPath;
        public ProgressPath progressPath;

        [Header("Misc")]
        public bool lockRotationToCamera;

        private bool doJump;
        private Vector3 move;
        public Vector3 inputDirection { get; private set; }
        private Vector3 moveDir;
        private Rigidbody rb;
        private Collider _collider;
        private Transform cam;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _collider = GetComponentInChildren<Collider>();
            cam = Camera.main.transform;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            UpdateLockRotationToCamera();
            inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            if (Input.GetButtonDown("Jump"))
            {
                // if (OnProgressPath)
                // {
                // ToggleProgressPath(false);
                // rb.velocity = Vector3.up * jumpVelocity;
                // }

                if (IsGrounded())
                {
                    if (OnProgressPath) ToggleProgressPath(false);
                    rb.velocity = Vector3.up * jumpVelocity;
                }
            }

            if (!jumping && rb.velocity.y > 0 && IsGrounded())
            {
                jumping = true;
                OnJumped.Invoke();
            }

            if (jumping && rb.velocity.y < 0 && IsGrounded())
            {
                jumping = false;
                OnLanded.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (OnProgressPath)
            {
                ProgressPathMove();
                return;
            }

            if (!OnProgressPath && inputDirection.magnitude > float.Epsilon)
            {
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);

                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                rb.velocity = new Vector3(moveDir.x * speed, rb.velocity.y, moveDir.z * speed);
            }
            else if (inputDirection.magnitude < float.Epsilon && IsGrounded()) rb.velocity = Vector3.Scale(rb.velocity, new Vector3(StopDampening, 1, StopDampening));

            MakeJumpNice();
        }

        public Quaternion GameplayDirection() => lockRotationToCamera ? cam.transform.rotation : transform.rotation;

        private void UpdateLockRotationToCamera()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) lockRotationToCamera = true;
            if (Input.GetKeyUp(KeyCode.LeftShift)) lockRotationToCamera = false;
        }

        public bool IsGrounded()
        {
            if (OnProgressPath) return true;
            return Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.1f, groundMask);
        }

        private void MakeJumpNice()
        {
            if (rb.velocity.y < 0)
            {
                var fallMult = slowFall ? slowFallMultiplier : fallMultiplier;
                rb.velocity += Vector3.up * (gravity * (fallMult - 1) * Time.deltaTime);
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.velocity += Vector3.up * (gravity * (lowJumpMultiplier - 1) * Time.deltaTime);
            }
        }

        public void ToggleProgressPath(bool value)
        {
            OnProgressPath = value;
            _collider.enabled = !value;
            rb.useGravity = !value;
        }

        public void ToggleProgressPath(bool value, ProgressPath path)
        {
            progressPath = path;
            ToggleProgressPath(value);
        }

        private void ProgressPathMove()
        {
            var verticalAxis = Input.GetAxis("Vertical");
            progressPath.Move(verticalAxis * speed * Time.deltaTime);
        }

        public void ToggleSlowFall(bool value) => slowFall = value;
    }
}