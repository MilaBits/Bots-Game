using System.Collections;
using System.Collections.Generic;
using Bots;
using UnityEngine;

public class CircleBotFormation : BotFormation
{
    private Coroutine _throwCoroutine;
    private float throwSpeed = 1f;
    private float throwHeight = 2f;
    private AnimationCurve _throwCurve;

    public CircleBotFormation(BotsController botsController) : base(botsController)
    {
        _throwCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, throwHeight), new Keyframe(1, 0));
        _throwCurve.keys[0].weightedMode = WeightedMode.None;
        _throwCurve.keys[2].weightedMode = WeightedMode.None;
    }

    public override List<Vector3> GetPositions(int botCount)
    {
        List<Vector3> positions = new List<Vector3>();
        float angle = 360f / botCount;
        Vector3 offset = new Vector3(0, 0, -_spacing);

        if (botCount < 5 && botCount > 1)
        {
            switch (botCount)
            {
                case 4:
                    positions.Add(Quaternion.Euler(0, +135, 0) * offset);
                    positions.Add(Quaternion.Euler(0, 45, 0) * offset);
                    positions.Add(Quaternion.Euler(0, -135, 0) * offset);
                    positions.Add(Quaternion.Euler(0, -45, 0) * offset);
                    break;
                case 3:
                    positions.Add(Quaternion.Euler(0, 60, 0) * offset);
                    positions.Add(Quaternion.Euler(0, 0, 0) * offset);
                    positions.Add(Quaternion.Euler(0, -60, 0) * offset);
                    break;
                case 2:
                    positions.Add(Quaternion.Euler(0, 30, 0) * offset);
                    positions.Add(Quaternion.Euler(0, -30, 0) * offset);
                    break;
            }

            return positions;
        }

        for (int i = 0; i < botCount; i++)
        {
            var pos = Quaternion.Euler(0, angle * i, 0) * offset;
            positions.Add(pos);
        }

        return positions;
    }

    public override void AttackStart()
    {
        _botsController.aiming = true;
    }

    public override void AttackEnd()
    {
        _botsController.aiming = false;
        if (_botsController.bots.Count < 1) return;
        if (_botsController.GetReadyBot(out var bot))
        {
            _botsController.StartCoroutine(Throw(bot, _botsController.Target.point));
        }
    }

    private IEnumerator Throw(Bot bot, Vector3 target)
    {
        bot.ToggleAgent(false);

        var startPos = bot.transform.position;
        for (float passedTime = 0; passedTime < throwSpeed; passedTime += Time.deltaTime)
        {
            bot.transform.position = Vector3.Lerp(startPos, target, passedTime / throwSpeed) + Vector3.up * _throwCurve.Evaluate(passedTime / throwSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        bot.ToggleAgent(true);
        _botsController.AddBot(bot);
    }
}