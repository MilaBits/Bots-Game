using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bots;
using UnityEngine;

public class FollowBotFormation : BotFormation
{
    private Coroutine _flipCoroutine;
    private Transform _flipPivot;
    private float _flipSpeed = .5f;

    public FollowBotFormation(BotsController botsController) : base(botsController)
    {
        _flipPivot = new GameObject("flipPivot").transform;
        _flipPivot.SetParent(botsController.transform, false);
        _flipPivot.localRotation = Quaternion.identity;
        _flipPivot.localPosition = Vector3.zero;
    }

    public override List<Vector3> GetPositions(int botCount)
    {
        float distance = _spacing;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 1; i < botCount + 1; i++)
        {
            var pos = Vector3.back * distance * i;
            positions.Add(pos);
        }

        return positions;
    }

    public override void AttackStart()
    {
        if (!_botsController.BotsReady()) return;

        foreach (Bot bot in _botsController.bots)
        {
            bot.transform.SetParent(_flipPivot);
            bot.ToggleAgent(false);
            bot.transform.LookAt(_flipPivot.transform.position + _flipPivot.transform.position);
        }

        _flipCoroutine = _botsController.StartCoroutine(Flip());
    }

    public IEnumerator Flip()
    {
        foreach (Bot bot in _botsController.bots) bot.CollisionEntered.AddListener(Hit);

        Quaternion startRot = Quaternion.identity;
        float timePassed = 0f;
        while (timePassed < _flipSpeed)
        {
            timePassed += Time.deltaTime;
            _flipPivot.localRotation = startRot * Quaternion.AngleAxis(timePassed / _flipSpeed * 180f, Vector3.right);

            yield return null;
        }

        foreach (Bot bot in _botsController.bots)
        {
            bot.CollisionEntered.RemoveListener(Hit);
        }

        ResetBots();

        _flipPivot.localRotation = startRot;
    }

    private void Hit(Bot hitBot, Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bot Trigger") && other.GetComponentInChildren<BridgeTrigger>())
        {
            _botsController.StopCoroutine(_flipCoroutine);
            foreach (Bot bot in _botsController.bots)
            {
                bot.CollisionEntered.RemoveListener(Hit);
                bot.transform.SetParent(null);
            }

            Vector3 firstBotPos = _botsController.bots.First().transform.position;
            Vector3 lastBotPos = _botsController.bots.Last().transform.position;
            _botsController.PlayerMovement.ToggleProgressPath(true, new ProgressPath(firstBotPos, lastBotPos, _botsController.transform));
            _botsController.PlayerMovement.OnLanded.AddListener(ReturnBotsToNormal);
        }
    }

    private void ReturnBotsToNormal()
    {
        _botsController.PlayerMovement.OnLanded.RemoveListener(ReturnBotsToNormal);
        foreach (Bot bot in _botsController.bots)
        {
            bot.transform.SetParent(null);
            bot.transform.position = _botsController.transform.position;
            bot.ToggleAgent(true);
        }
    }
}