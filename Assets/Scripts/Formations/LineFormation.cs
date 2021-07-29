using System.Collections;
using System.Collections.Generic;
using Bots;
using UnityEngine;

public class LineBotFormation : BotFormation
{
    private Coroutine _spinCoroutine;
    private Transform _spinPivot;
    private float _spinSpeed = 1;

    public LineBotFormation(BotsController botsController) : base(botsController)
    {
        _spinPivot = new GameObject("spinPivot").transform;
        _spinPivot.SetParent(botsController.transform);
        _spinPivot.localPosition = Vector3.zero;
    }

    public override List<Vector3> GetPositions(int botCount)
    {
        float distance = _spacing;
        List<Vector3> positions = new List<Vector3>();
        for (int i = 1; i < botCount + 1; i++)
        {
            var pos = Vector3.right * distance;
            if (i % 2 == 0)
            {
                pos *= -1;
                distance += _spacing;
            }

            positions.Add(pos);
        }

        return positions;
    }

    public IEnumerator Spin()
    {
        Quaternion startRot = _spinPivot.rotation;
        for (float passedTime = 0; passedTime < _spinSpeed; passedTime += Time.deltaTime)
        {
            _spinPivot.rotation = startRot * Quaternion.AngleAxis(passedTime / -_spinSpeed * 360f, Vector3.up);
            yield return null;
        }

        _spinCoroutine = _botsController.StartCoroutine(Spin());
        _spinPivot.rotation = startRot;
    }

    public override void AttackStart()
    {
        if (!_botsController.BotsReady() || !_botsController.PlayerMovement.IsGrounded()) return;
        foreach (Bot bot in _botsController.bots)
        {
            bot.transform.SetParent(_spinPivot);
            bot.ToggleAgent(false);
            bot.transform.LookAt(_spinPivot.transform.position + _spinPivot.transform.position);
        }

        _botsController.GetComponent<PlayerMovement>().ToggleSlowFall(true);
        _spinCoroutine = _botsController.StartCoroutine(Spin());
    }

    public override void AttackEnd()
    {
        base.AttackEnd();

        _botsController.StopCoroutine(_spinCoroutine);
        foreach (Bot bot in _botsController.bots)
        {
            bot.transform.SetParent(null);
            bot.ToggleAgent(true);
        }

        _botsController.PlayerMovement.ToggleSlowFall(false);
    }
}