using UnityEngine;

namespace Client.Gameplay.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public partial class SimpleRider : MonoBehaviour
    {
        [Header("Motor Body")]
        [SerializeField] private Rigidbody _motorBody;

        [Header("Move")]
        [SerializeField, Tooltip("Target horizontal speed m/s")]
        private float _moveSpeed = 6f;

        [SerializeField, Tooltip("Accel when input present")]
        private float _accel = 30f;

        [SerializeField, Tooltip("Decel when input is zero")]
        private float _decel = 40f;

        [SerializeField, Tooltip("Hard horizontal cap")]
        private float _maxSpeed = 8f;

        [Header("Turn")]
        [SerializeField, Tooltip("Deg per second")]
        private float _turnSpeed = 360f;

        [Header("Target")]
        [SerializeField] private Transform _lookTarget;

        private Transform _tr;
        private Vector3 _moveInput;

        private void Awake()
        {
            _tr = transform;
            if (_motorBody == null)
            {
                _motorBody = GetComponent<Rigidbody>();
            }

            _motorBody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void FixedUpdate()
        {
            ApplyMovementXZ();
            ApplyRotationY();
        }

        private void ApplyMovementXZ()
        {
            var velocity = _motorBody.linearVelocity;
            var horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

            var want = _moveInput * _moveSpeed;
            var hasInput = _moveInput.sqrMagnitude > 1e-6f;

            var rate = (hasInput ? _accel : _decel) * Time.fixedDeltaTime;
            var delta = want - horizontalVelocity;

            // clamp per-step acceleration on XZ
            if (delta.sqrMagnitude > rate * rate)
            {
                delta = delta.normalized * rate;
            }

            var newHorizontalVelocity = horizontalVelocity + delta;

            // clamp horizontal magnitude
            var mag = newHorizontalVelocity.magnitude;
            if (mag > _maxSpeed)
            {
                newHorizontalVelocity *= (_maxSpeed / mag);
            }

            _motorBody.linearVelocity = new Vector3(newHorizontalVelocity.x, velocity.y, newHorizontalVelocity.z);
        }

        private void ApplyRotationY()
        {
            var heading = Vector3.zero;

            // Prefer target if present
            if (_lookTarget != null)
            {
                var direction = _lookTarget.position - _tr.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 1e-6f)
                {
                    heading = direction;
                }
            }

            // Fall back to input
            if (heading == Vector3.zero && _moveInput.sqrMagnitude > 1e-6f)
            {
                heading = new Vector3(_moveInput.x, 0f, _moveInput.z);
            }

            // Final fallback to current horizontal velocity
            if (heading == Vector3.zero)
            {
                var velocity = _motorBody.linearVelocity;
                velocity.y = 0f;
                if (velocity.sqrMagnitude > 1e-4f)
                {
                    heading = velocity;
                }
            }

            if (heading == Vector3.zero)
            {
                return;
            }

            var targetRot = Quaternion.LookRotation(heading, Vector3.up);
            var maxStep = _turnSpeed * Time.fixedDeltaTime;
            _tr.rotation = Quaternion.RotateTowards(_tr.rotation, targetRot, maxStep);
        }

        public void Move(Vector3 direction)
        {
            direction.y = 0f;
            _moveInput = Vector3.ClampMagnitude(direction, 1f);
        }

        public void SetLookTarget(Transform t)
        {
            _lookTarget = t;
        }
    }
}