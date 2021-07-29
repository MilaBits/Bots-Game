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

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.stoppingDistance = .01f;
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
            if (_agent.enabled) _agent.speed = defaultMoveSpeed + _agent.remainingDistance;
        }

        public void ToggleAgent(bool value)
        {
            _agent.enabled = value;
            Debug.Log("test");
            if (value && _agent.isOnNavMesh) transform.position = _agent.destination;
        }

        public bool IsReady()
        {
            if (_agent.enabled) return _agent.remainingDistance < .01f;
            return true;
        }

        private void OnTriggerEnter(Collider other) => CollisionEntered.Invoke(this, other);
    }
}