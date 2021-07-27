using System.Collections.Generic;
using UnityEngine;

namespace Bots
{
    public class BotsController : MonoBehaviour
    {
        public Queue<Bot> bots = new Queue<Bot>();
        public Bot botPrefab;
        private BotWaypoints _waypoints;
        public PlayerMovement PlayerMovement;

        public bool aiming;
        private GameObject _targeter;
        public GameObject targeterPrefab;
        public RaycastHit Target;

        private void Start()
        {
            _targeter = Instantiate(targeterPrefab);
            _waypoints = GetComponent<BotWaypoints>();
            PlayerMovement = GetComponent<PlayerMovement>();
            _waypoints.SetFormations(new List<BotFormation> {new CircleBotFormation(this), new LineBotFormation(this), new FollowBotFormation(this)});
            _waypoints.NextFormation();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) InitializeBot();
            if (Input.GetKeyDown(KeyCode.Mouse0)) _waypoints.activeFormation.AttackStart();
            if (Input.GetKeyUp(KeyCode.Mouse0)) _waypoints.activeFormation.AttackEnd();

            var positions = _waypoints.activeFormation.GetPositions(bots.Count);
            int index = 0;
            foreach (Bot bot in bots)
            {
                bot.SetTarget(transform.position + transform.rotation * positions[index]);
                index++;
            }

            _targeter.SetActive(aiming);
            if (aiming)
            {
                var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Target = hit;
                    _targeter.transform.position = hit.point;
                    _targeter.transform.up = hit.normal;
                }
            }
        }

        public bool BotsReady()
        {
            foreach (Bot bot in bots)
            {
                if (!bot.IsReady())
                {
                    return false;
                }
            }

            return true;
        }

        public void InitializeBot()
        {
            var bot = Instantiate(botPrefab, transform.position + -(transform.forward * 5), Quaternion.identity);
            AddBot(bot);
        }

        public void AddBot(Bot bot)
        {
            // bot.SetWaypointTarget(_waypoints, _waypoints.botCount);
            bots.Enqueue(bot);
            _waypoints.botCount++;
        }

        public Bot GetBot()
        {
            _waypoints.botCount--;
            return bots.Dequeue();
        }
    }
}