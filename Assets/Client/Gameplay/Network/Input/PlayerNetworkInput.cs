using System.Collections.Generic;
using Client.Gameplay.Movement;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput : NetworkBehaviour
    {
        private const int   MAX_PENDING_INPUTS = 120; // 2 seconds with 60 FPS
        private const float SIM_DT             = 1f / 60f;

        [SerializeField] private SimpleRider _rider;

        [SerializeField, Range(1, 120)] private int _sendingTargetFPS = 30;

        private readonly List<InputSnapshot> _pending = new();

        private bool                     _isHost;
        private uint                     _seq;
        private float                    _accumLocal;
        private float                    _accumSend;
        private float                    _sendDelta;
        private Transform                _tr;
        private GameplayContextBehaviour _gameplayContext;
        private int                      _lastSentIndex;

        private void Awake()
        {
            _sendDelta = 1f / _sendingTargetFPS;
            _tr = transform;
            _gameplayContext = GameplayContextBehaviour.Instance;
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _isHost = IsServerInitialized;
        }

        private void FixedUpdate()
        {
            if (IsOwner || _isHost)
            {
                UpdateLookAt();
            }
        }

        public void CaptureLocalInput(Vector2 direction, float deltaTime, byte flags)
        {
            if (!IsOwner)
            {
                return;
            }

            _accumLocal += deltaTime;
            while (_accumLocal >= SIM_DT)
            {
                var inputSnapshot = new InputSnapshot
                {
                    Sequence = ++_seq,
                    Direction = direction,
                    Flags = flags
                };

                if (!_isHost)
                {
                    PredictLocal(in inputSnapshot);
                }

                _pending.Add(inputSnapshot);
                _accumLocal -= SIM_DT;
            }

            _accumSend += deltaTime;
            if (_accumSend < _sendDelta)
            {
                return;
            }

            _accumSend = 0f;
            if (_pending.Count <= _lastSentIndex)
            {
                return;
            }

            var toSend = _pending.GetRange(_lastSentIndex, _pending.Count - _lastSentIndex);
            SubmitInputServerRpc(toSend.ToArray());

            if (_pending.Count > MAX_PENDING_INPUTS)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Too many pending inputs ({_pending.Count}), connection issues?");
#endif
                _pending.RemoveRange(0, _pending.Count - MAX_PENDING_INPUTS);
            }

            _lastSentIndex = _pending.Count;
        }

        private void PredictLocal(in InputSnapshot s)
        {
            _rider.SimulateStep(s.Direction, SIM_DT);
        }

        private void UpdateLookAt()
        {
            if (_gameplayContext.TryFindNearestEnemy(_tr.position, out var _,
                    out var nearestEntity))
            {
                _rider.SetLookTarget(nearestEntity.transform);
            }
            else
            {
                _rider.SetLookTarget(null);
            }
        }
    }
}