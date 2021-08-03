using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bots
{
    public class BotsController : MonoBehaviour
    {
        public Queue<Bot> bots = new Queue<Bot>();
        public Bot botPrefab;
        private BotWaypoints _waypoints;
        public ThirdPersonMovement PlayerMovement;

        public bool aiming;
        private GameObject _targeter;
        public GameObject targeterPrefab;
        public RaycastHit Target;

        private void Start()
        {
            _targeter = Instantiate(targeterPrefab);
            _waypoints = GetComponent<BotWaypoints>();
            PlayerMovement = GetComponent<ThirdPersonMovement>();
            _waypoints.SetFormations(new List<BotFormation> {new CircleBotFormation(this), new LineBotFormation(this), new FollowBotFormation(this)});
            _waypoints.OnFormationChanged.AddListener(UpdateBotTargets);
            _waypoints.NextFormation();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) InitializeBot();
            if (Input.GetKeyDown(KeyCode.Mouse0)) _waypoints.activeFormation.AttackStart();
            if (Input.GetKeyUp(KeyCode.Mouse0)) _waypoints.activeFormation.AttackEnd();
            if (PlayerMovement.inputDirection.magnitude >= float.Epsilon || PlayerMovement.lockRotationToCamera) UpdateBotTargets();

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

        public void UpdateBotTargets()
        {
            var positions = _waypoints.activeFormation.GetPositions(bots.Count);
            int index = 0;
            foreach (Bot bot in bots)
            {
                bot.SetTarget(transform.position + PlayerMovement.GameplayDirection() * positions[index]);
                index++;
            }
        }

        public bool BotsReady()
        {
            foreach (Bot bot in bots)
                if (!bot.IsReady())
                    return false;
            return true;
        }

        public void InitializeBot()
        {
            var bot = Instantiate(botPrefab, transform.position + -(transform.forward * 5), Quaternion.identity);
            bot.GetComponent<NavMeshAgent>().speed = PlayerMovement.speed;
            AddBot(bot);
        }

        public void AddBot(Bot bot)
        {
            bots.Enqueue(bot);
            _waypoints.botCount++;
            UpdateBotTargets();
        }

        public Bot GetBot()
        {
            _waypoints.botCount--;
            Bot bot = bots.Dequeue();
            UpdateBotTargets();
            return bot;
        }

        public bool GetReadyBot(out Bot bot)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                Bot currentBot = GetBot();
                if (currentBot.IsReady())
                {
                    bot = currentBot;
                    return true;
                }

                AddBot(currentBot);
            }

            bot = null;
            return false;
        }
    }
}