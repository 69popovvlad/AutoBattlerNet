using System.Collections;
using Client.Services.Injections;
using UnityEngine;

namespace Client.Gameplay.Camera
{
    /// <summary>
    /// Simple follow camera tacks a target on XZ plane
    /// </summary>
    public class CameraFollower : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField, Range(0.01f, 1f)] private float smoothTime = 0.2f;

        [Tooltip("Half-size of central box in world units.")]
        [SerializeField] private Vector2 deadZoneSize = new Vector2(1.5f, 1.0f);

        [Tooltip("Offset from target in world space.")]
        [SerializeField] private Vector3 offset = new Vector3(0, 10, -8);

        private ICameraTargetProvider _targetProvider;
        private Vector3 _velocity;

        private void Awake()
        {
            _targetProvider = Ioc.Instance.Resolve<ICameraTargetProvider>();
        }

        private void LateUpdate()
        {
            UpdateFollow();
        }

        private void UpdateFollow()
        {
            var target = _targetProvider?.CameraTarget;
            if (target == null)
            {
                return;
            }

            var currentPos = transform.position;
            var desiredPos = target.position + offset;

            // restrict to XZ plane (keep camera Y)
            desiredPos.y = currentPos.y;

            var delta = desiredPos - currentPos;

            // dead zone logic (ignore tiny movement)
            if (Mathf.Abs(delta.x) < deadZoneSize.x && Mathf.Abs(delta.z) < deadZoneSize.y)
            {
                return;
            }

            // smooth move
            var newPos = Vector3.SmoothDamp(currentPos, desiredPos, ref _velocity, smoothTime);
            transform.position = newPos;
        }
    }
}