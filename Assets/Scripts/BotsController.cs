using System.Collections.Generic;
using UnityEngine;

namespace Bots
{
    public class BotsController : MonoBehaviour
    {
        public Queue<Bot> bots = new Queue<Bot>();
        public Bot botPrefab;
        private BotWaypoints _waypoints;


        private void Start()
        {
            _waypoints = GetComponent<BotWaypoints>();
            _waypoints.SetFormations(new List<BotFormation> {new CircleBotFormation(this), new LineBotFormation(this), new FollowBotFormation(this)});
            _waypoints.NextFormation();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) InitializeBot();
            if (Input.GetKeyDown(KeyCode.Mouse0) && BotsReady()) _waypoints.activeFormation.AttackStart();
            if (Input.GetKeyUp(KeyCode.Mouse0)) _waypoints.activeFormation.AttackEnd();

            var positions = _waypoints.activeFormation.GetPositions(bots.Count);
            int index = 0;
            foreach (Bot bot in bots)
            {
                bot.SetTarget(transform.position + transform.rotation * positions[index]);
                index++;
            }
        }

        private bool BotsReady()
        {
            foreach (Bot bot in bots)
            {
                if (!bot.IsReady()) return false;
            }

            return true;
        }

        public void InitializeBot()
        {
            var bot = Instantiate(botPrefab, Vector3.zero, Quaternion.identity);
            AddBot(bot);
        }

        public void AddBot(Bot bot)
        {
            bot.SetWaypointTarget(_waypoints, _waypoints.botCount);
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