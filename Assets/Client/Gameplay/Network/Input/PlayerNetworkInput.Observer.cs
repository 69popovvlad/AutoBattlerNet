using System.Collections.Generic;
using Client.Gameplay.Movement;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput
    {
        [Header("Observer Interpolation")]
        [SerializeField, Tooltip("Buffer size for smooth interpolation")]
        private int _observerBufferSize = 3;

        [SerializeField, Tooltip("Interpolation delay in seconds")]
        private float _observerInterpolationDelay = 0.1f;

        private readonly Queue<(float time, PlayerState state)> _observerBuffer = new();
        private          PlayerState                            _observerTargetState;
        private          PlayerState                            _observerFromState;
        private          float                                  _observerLerpTime;

        [ObserversRpc(ExcludeOwner = true)]
        private void StateForObserversRpc(PlayerState playerState)
        {
            if (IsOwner || IsServerInitialized)
            {
                return;
            }

            _observerBuffer.Enqueue((Time.time, playerState));

            while (_observerBuffer.Count > _observerBufferSize)
            {
                _observerBuffer.Dequeue();
            }
        }

        private void Update()
        {
            if (IsOwner || _observerBuffer.Count < 2)
            {
                return;
            }

            // Interpolation with delay for smoothing
            var targetTime = Time.time - _observerInterpolationDelay;

            // Find the two closest states
            var states = _observerBuffer.ToArray();
            for (int i = 0, ilen = states.Length; i < ilen - 1; ++i)
            {
                var (time0, state0) = states[i];
                var (time1, state1) = states[i + 1];

                if (targetTime < time0 || targetTime > time1)
                {
                    continue;
                }

                // Interpolate between these states
                var t = Mathf.InverseLerp(time0, time1, targetTime);

                var interpolated = new KinematicState
                {
                    Position = Vector3.Lerp(state0.Position, state1.Position, t),
                    Velocity = Vector3.Lerp(state0.Velocity, state1.Velocity, t),
                    Yaw = Mathf.LerpAngle(state0.Yaw, state1.Yaw, t)
                };

                _rider.SetState(interpolated);
                break;
            }

            // Clear old states
            while (_observerBuffer.Count > 0 && _observerBuffer.Peek().time < targetTime - 1f)
            {
                _observerBuffer.Dequeue();
            }
        }
    }
}