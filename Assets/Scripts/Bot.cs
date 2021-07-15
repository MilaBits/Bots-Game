using UnityEngine;
using UnityEngine.AI;

namespace Bots
{
    public class Bot : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private int _id;

        private BotWaypoints _waypoints;

        private bool follow;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = 10f;
            _agent.angularSpeed = 1440f;
            _agent.acceleration = 60f;
        }

        public void SetWaypointTarget(BotWaypoints waypoints, int id)
        {
            _id = id;
            _waypoints = waypoints;
            follow = true;
        }

        public void SetTarget(Vector3 target)
        {
            if (_agent.enabled) _agent.destination = target;
        }

        private void Update() { }

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