using System.Collections.Generic;
using Client.Gameplay.Movement;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput : NetworkBehaviour
    {
        private const float SIM_DT = 1f / 60f;

        [SerializeField] private SimpleRider _rider;

        [SerializeField, Range(1, 120)] private int _sendingTargetFPS = 30;

        private readonly List<InputSnapshot> _pending = new();

        private bool _isHost;
        private uint _seq;
        private float _accumLocal;
        private float _accumSend;
        private float _sendDelta;
        private Transform _tr;
        private GameplayContextBehaviour _gameplayContext;

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

        public void CaptureLocalInput(Vector2 direction, float dt, byte flags)
        {
            UpdateLookAt();

            if (!IsOwner)
            {
                return;
            }

            _accumLocal += dt;
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

            _accumSend += dt;
            if (_accumSend < _sendDelta)
            {
                return;
            }

            _accumSend = 0f;
            if (_pending.Count > 0)
            {
                SubmitInputServerRpc(_pending.ToArray());
            }
        }

        private void PredictLocal(in InputSnapshot s)
        {
            _rider.ApplyInputStep(s.Direction, SIM_DT);
        }

        private void UpdateLookAt()
        {
            if (_gameplayContext.TryFindNearestEnemy(_tr.position, out var _,
                    out var nearestEntityGhost))
            {
                _rider.SetLookTarget(nearestEntityGhost.transform);
            }
            else
            {
                _rider.SetLookTarget(null);
            }
        }
    }
}