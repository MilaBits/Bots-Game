using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bots
{
    public class BotWaypoints : MonoBehaviour
    {
        public BotFormation activeFormation;
        public int botCount;

        private Queue<BotFormation> _formations = new Queue<BotFormation>();

        public UnityEvent OnFormationChanged = new UnityEvent();

        public void SetFormations(List<BotFormation> formations) => _formations = new Queue<BotFormation>(formations);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) NextFormation();
        }

        public void NextFormation()
        {
            if (activeFormation != null)
            {
                activeFormation.Deactivate();
                _formations.Enqueue(activeFormation);
            }

            activeFormation = _formations.Dequeue();
            activeFormation.Activate();
            OnFormationChanged.Invoke();
        }
    }
}