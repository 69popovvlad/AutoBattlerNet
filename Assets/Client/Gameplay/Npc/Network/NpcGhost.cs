using Client.Gameplay.Movement;
using UnityEngine;
using Validosik.Client.Character;

namespace Client.Gameplay.Npc.Network
{
    public class NpcGhost : MonoBehaviour
    {
        [SerializeField] private SimpleRider _rider;

        [Header("Smoothing")]
        [SerializeField, Tooltip("gentle pull to the state")]
        private float _nudgeThreshold = 0.05f;

        [SerializeField, Tooltip("set the state immediately")]
        private float _snapThreshold = 0.50f;

        [SerializeField, Range(0f, 1f)] private float _nudgeVelFactor = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _nudgePosFactor = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _nudgeYawFactor = 0.5f;

        private uint _lastAppliedTick;

        public void ApplyServerState(in NpcState state)
        {
            if (unchecked((int)(state.Tick - _lastAppliedTick)) <= 0)
            {
                return;
            }

            _lastAppliedTick = state.Tick;

            var kinematicState = _rider.GetState();
            var positionError = state.Position - kinematicState.Position;
            var sqrMagnitudeError = positionError.sqrMagnitude;

            var nudge2 = _nudgeThreshold * _nudgeThreshold;
            var snap2 = _snapThreshold * _snapThreshold;

            if (sqrMagnitudeError >= snap2)
            {
                _rider.SetState(new KinematicState
                {
                    Position = state.Position,
                    Velocity = state.Velocity,
                    Yaw = state.Yaw
                });
                return;
            }

            if (sqrMagnitudeError >= nudge2)
            {
                kinematicState.Position += positionError * _nudgePosFactor;
                kinematicState.Velocity = Vector3.Lerp(kinematicState.Velocity, state.Velocity, _nudgeVelFactor);
                var yawErr = Mathf.DeltaAngle(kinematicState.Yaw, state.Yaw);
                kinematicState.Yaw += yawErr * _nudgeYawFactor;
                _rider.SetState(kinematicState);
                return;
            }

            // Small error = nothing to do
        }
    }
}