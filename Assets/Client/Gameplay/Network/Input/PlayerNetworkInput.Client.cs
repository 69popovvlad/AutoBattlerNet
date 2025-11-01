using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Validosik.Client.Character;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput
    {
        [Header("CLIENT")]
        [SerializeField, Tooltip("gentle pull to the state")]
        private float nudgeThreshold = 0.05f;

        [SerializeField, Tooltip("set the state immediately")]
        private float snapThreshold = 0.50f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float nudgeVelFactor = 0.5f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float nudgePosFactor = 0.5f;

        [SerializeField, Range(0f, 1f), Tooltip("delta to pull variable")]
        private float nudgeYawFactor = 0.5f;

        [TargetRpc]
        private void AckTargetRpc(NetworkConnection owner, PlayerState s)
        {
            // Remove old snapshots
            var i = _pending.FindLastIndex(x => x.Sequence == s.LastSequence);
            if (i >= 0)
            {
                _pending.RemoveRange(0, i + 1);
            }

            // We have already run a simulation on the host
            if (_isHost)
            {
                return;
            }

            // Correction on owner's client
            var kinematicState = _rider.GetState();
            var positionError = s.Position - kinematicState.Position;
            var sqrError = positionError.sqrMagnitude;

            var nudgeSqrError = nudgeThreshold * nudgeThreshold;
            var snapSqrError = snapThreshold * snapThreshold;

            // Snap state
            if (sqrError >= snapSqrError)
            {
                _rider.SetState(new KinematicState
                {
                    Position = s.Position,
                    Velocity = s.Velocity,
                    Yaw = s.Yaw
                });
            }
            // Nudge
            else if (sqrError >= nudgeSqrError)
            {
                kinematicState.Position += positionError * nudgePosFactor;
                kinematicState.Velocity = Vector3.Lerp(kinematicState.Velocity, s.Velocity, nudgeVelFactor);

                var yawErr = Mathf.DeltaAngle(kinematicState.Yaw, s.Yaw);
                kinematicState.Yaw += yawErr * nudgeYawFactor;

                _rider.SetState(kinematicState);
            }

            // Or do nothing if error is too small 
            // Just predict locally
            foreach (var inputSnapshot in _pending)
            {
                PredictLocal(in inputSnapshot);
            }
        }
    }
}