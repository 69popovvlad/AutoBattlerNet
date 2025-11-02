using UnityEngine;
using Validosik.Client.Character;

namespace Client.Gameplay.Movement
{
    public partial class SimpleRider : ICharacterKinematics
    {
        public void ApplyInputStep(Vector2 direction, float delta)
        {
            var v = new Vector3(direction.x, 0f, direction.y);
            _moveInput = Vector3.ClampMagnitude(v, 1f);
        }

        public KinematicState GetState()
        {
            return new KinematicState
            {
                Position = _tr.position,
                Velocity = _motorBody.linearVelocity,
                Yaw = _tr.eulerAngles.y
            };
        }

        public void SetState(in KinematicState state)
        {
            _motorBody.position = state.Position;
            _motorBody.linearVelocity = state.Velocity;
            var e = _tr.eulerAngles;
            e.y = state.Yaw;
            _tr.eulerAngles = e;
        }
    }
}