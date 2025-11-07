using Client.Gameplay.Movement;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput
    {
        [Header("CLIENT")]
        [SerializeField, Tooltip("gentle pull to the state")]
        private float _nudgeThreshold = 0.05f;

        [SerializeField, Tooltip("set the state immediately")]
        private float _snapThreshold = 0.50f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float _nudgeVelFactor = 0.5f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float _nudgePosFactor = 0.5f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float _nudgeYawFactor = 0.5f;

        [TargetRpc]
        private void AckTargetRpc(NetworkConnection owner, PlayerState s)
        {
            var confirmedIndex = _pending.FindLastIndex(x => x.Sequence == s.LastSequence);

            if (_isHost) // Never reached in theory
            {
                var hostIndex = confirmedIndex;
                if (hostIndex >= 0)
                {
                    _pending.RemoveRange(0, hostIndex + 1);
                }

                return;
            }


            // If there is no such input anymore (ACK is too old), ignore it
            if (confirmedIndex < 0)
            {
                return;
            }

            var currentState = _rider.GetState();

            // Roll back to the server state
            _rider.SetState(new KinematicState
            {
                Position = s.Position,
                Velocity = s.Velocity,
                Yaw = s.Yaw
            });

            // Replay all inputs after confirmed
            for (int i = confirmedIndex + 1, ilen = _pending.Count; i < ilen; ++i)
            {
                PredictLocal(_pending[i]);
            }

            var replayedState = _rider.GetState();

            // Calculate the error between the current and replayed
            var positionError = currentState.Position - replayedState.Position;
            var sqrError = positionError.sqrMagnitude;

            var nudgeSqrError = _nudgeThreshold * _nudgeThreshold;
            var snapSqrError = _snapThreshold * _snapThreshold;

            // If the error is large, we apply correction
            if (sqrError >= snapSqrError)
            {
                // The correct state is already set after replay
#if UNITY_EDITOR
                Debug.Log($"[Reconciliation] SNAP error: {Mathf.Sqrt(sqrError):F3}m");
#endif
            }
            else if (sqrError >= nudgeSqrError)
            {
                // Smoothly pull to the correct position
                var correctedState = replayedState;
                correctedState.Position =
                    Vector3.Lerp(replayedState.Position, currentState.Position, 1f - _nudgePosFactor);
                correctedState.Velocity =
                    Vector3.Lerp(replayedState.Velocity, currentState.Velocity, 1f - _nudgeVelFactor);

                var yawErr = Mathf.DeltaAngle(replayedState.Yaw, currentState.Yaw);
                correctedState.Yaw = replayedState.Yaw + yawErr * (1f - _nudgeYawFactor);

                _rider.SetState(correctedState);

                Debug.Log($"[Reconciliation] NUDGE error: {Mathf.Sqrt(sqrError):F3}m");
            }
            else
            {
                // Leave the replayed state as is
            }

            // Remove confirmed inputs
            _pending.RemoveRange(0, confirmedIndex + 1);
            _lastSentIndex = 0;
        }
    }
}