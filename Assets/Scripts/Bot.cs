using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Bots
{
    public class Bot : MonoBehaviour
    {
        private NavMeshAgent _agent;
        public float defaultMoveSpeed = 6f;
        public UnityEvent<Bot, Collider> CollisionEntered = new UnityEvent<Bot, Collider>();

        [SerializeField] private DebugWorldSpaceUI _debugWorldSpaceUI;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = defaultMoveSpeed;
            _agent.angularSpeed = 1440f;
            _agent.acceleration = 60f;
        }

        public void SetTarget(Vector3 target)
        {
            if (_agent.enabled) _agent.destination = target;
        }

        private void Update()
        {
            if (_debugWorldSpaceUI.isActiveAndEnabled)
            {
                bool ready = IsReady();
                _debugWorldSpaceUI.Text = $"Ready: <color={(ready ? "green" : "red")}>{ready}</color>";
            }
        }

        public void ToggleAgent(bool value)
        {
            _agent.enabled = value;
            if (value && _agent.isOnNavMesh) transform.position = _agent.destination;
        }

        public bool IsReady()
        {
            if (_agent.enabled
                && !_agent.pathPending
                && _agent.remainingDistance <= _agent.stoppingDistance
                && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f))
                return true;
            return false;
        }

        private void OnTriggerEnter(Collider other) => CollisionEntered.Invoke(this, other);
    }
}