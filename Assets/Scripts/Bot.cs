using UnityEngine;
using UnityEngine.AI;

namespace Bots
{
    public class Bot : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private NavMeshAgent _agent;
        private bool follow = true;

        private bool wasReady;

        [SerializeField] private Material readyMaterial;
        [SerializeField] private Material movingMaterial;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = 10f;
            _agent.angularSpeed = 1440f;
            _agent.acceleration = 60f;
        }

        public void SetTarget(Vector3 target)
        {
            if (_agent.enabled) _agent.destination = target;
        }

        private void Update()
        {
            if (follow)
            {
                if (wasReady != IsReady()) _meshRenderer.material = IsReady() ? readyMaterial : movingMaterial;
                wasReady = IsReady();
            }
        }

        public void ToggleAgent(bool value)
        {
            follow = value;
            _agent.enabled = value;
        }

        public bool IsReady()
        {
            return _agent.remainingDistance < .01f;
        }
    }
}