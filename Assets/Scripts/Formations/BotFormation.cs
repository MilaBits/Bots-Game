using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bots
{
    [Serializable]
    public abstract class BotFormation
    {
        protected BotsController _botsController;
        protected float _spacing = 1.1f;

        public BotFormation(BotsController botsController)
        {
            _botsController = botsController;
        }

        public abstract List<Vector3> GetPositions(int botCount);

        public virtual void AttackStart() { }
        public virtual void AttackEnd() { }

        public virtual void Activate() { }
        public virtual void Deactivate() { }


        public void ResetBots()
        {
            int index = 0;
            var positions = GetPositions(_botsController.bots.Count);
            foreach (Bot bot in _botsController.bots)
            {
                bot.transform.SetParent(null);
                bot.ToggleAgent(true);
                bot.SetTarget(_botsController.transform.position + _botsController.transform.rotation * positions[index]);
                index++;
            }
        }
    }
}