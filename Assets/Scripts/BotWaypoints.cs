using System.Collections.Generic;
using UnityEngine;

namespace Bots
{
    public class BotWaypoints : MonoBehaviour
    {
        public BotFormation activeFormation;
        public int botCount;

        private Queue<BotFormation> _formations = new Queue<BotFormation>();

        public void SetFormations(List<BotFormation> formations) => _formations = new Queue<BotFormation>(formations);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) NextFormation();
        }

        public void NextFormation()
        {
            if (activeFormation != null) _formations.Enqueue(activeFormation);
            activeFormation = _formations.Dequeue();
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.green;
        //     foreach (var position in activeFormation.GetPositions(botCount))
        //     {
        //         Gizmos.DrawSphere(transform.position + position, .1f);
        //     }
        // }

        // public Vector3 GetPosition(Bot bot) => transform.position + transform.rotation * activeFormation.GetPositions(botCount)[id];
    }
}