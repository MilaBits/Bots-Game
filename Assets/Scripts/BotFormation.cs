using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bots
{
    [Serializable]
    public abstract class BotFormation
    {
        protected BotsController _botsController;
        protected float _spacing = 1f;

        public BotFormation(BotsController botsController)
        {
            _botsController = botsController;
        }

        public abstract List<Vector3> GetPositions(int botCount);

        public abstract void AttackStart();
        public abstract void AttackEnd();
    }

    public class CircleBotFormation : BotFormation
    {
        private Coroutine _aimCoroutine;
        private Coroutine _throwCoroutine;
        private bool _aiming;
        private bool _throwing;
        private GameObject _targeter;
        private float throwSpeed = 1f;
        private float throwHeight = 2f;
        private AnimationCurve _throwCurve;

        public CircleBotFormation(BotsController botsController) : base(botsController)
        {
            _targeter = GameObject.Instantiate(Resources.Load<GameObject>("Target"));
            _throwCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, throwHeight), new Keyframe(1, 0));
            _throwCurve.keys[0].weightedMode = WeightedMode.None;
            _throwCurve.keys[2].weightedMode = WeightedMode.None;
            _targeter.SetActive(false);
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
            _aimCoroutine = _botsController.StartCoroutine(Aim());
        }

        public override void AttackEnd()
        {
            _targeter.SetActive(false);

            if (_botsController.bots.Count < 1) return;
            _botsController.StartCoroutine(Throw(_targeter.transform.position));
        }

        IEnumerator Aim()
        {
            _targeter.SetActive(true);
            while (_targeter.activeSelf)
            {
                var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    _targeter.transform.position = hit.point;
                    _targeter.transform.up = hit.normal;
                }

                yield return null;
            }
        }

        private IEnumerator Throw(Vector3 target)
        {
            Bot bot = _botsController.GetBot();
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

            _spinPivot.rotation = startRot;
            _spinCoroutine = _botsController.StartCoroutine(Spin());
        }

        public override void AttackStart()
        {
            foreach (Bot bot in _botsController.bots)
            {
                bot.transform.SetParent(_spinPivot);
                bot.ToggleAgent(false);
                bot.transform.LookAt(_spinPivot.transform.position + _spinPivot.transform.position);
            }

            _spinCoroutine = _botsController.StartCoroutine(Spin());
        }

        public override void AttackEnd()
        {
            _botsController.StopCoroutine(_spinCoroutine);
            foreach (Bot bot in _botsController.bots)
            {
                bot.transform.SetParent(null);
                bot.ToggleAgent(true);
            }
        }
    }

    public class FollowBotFormation : BotFormation
    {
        private Coroutine _flipCoroutine;
        private Transform _flipPivot;
        private float _flipSpeed = .5f;

        public FollowBotFormation(BotsController botsController) : base(botsController)
        {
            _flipPivot = new GameObject("flipPivot").transform;
            _flipPivot.SetParent(botsController.transform);
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
            foreach (Bot bot in _botsController.bots)
            {
                bot.transform.SetParent(_flipPivot);
                bot.ToggleAgent(false);
                bot.transform.LookAt(_flipPivot.transform.position + _flipPivot.transform.position);
            }

            _flipCoroutine = _botsController.StartCoroutine(Flip());
        }

        public override void AttackEnd() { }

        public IEnumerator Flip()
        {
            Quaternion startRot = _flipPivot.rotation;
            float timePassed = 0f;
            while (timePassed < _flipSpeed)
            {
                timePassed += Time.deltaTime;
                _flipPivot.rotation = startRot * Quaternion.AngleAxis(timePassed / -_flipSpeed * -180f, Vector3.right);
                yield return null;
            }

            foreach (Bot bot in _botsController.bots)
            {
                bot.transform.SetParent(null);
                bot.ToggleAgent(true);
            }
        }
    }
}