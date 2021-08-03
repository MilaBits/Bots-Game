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
    }

    

    

    
}