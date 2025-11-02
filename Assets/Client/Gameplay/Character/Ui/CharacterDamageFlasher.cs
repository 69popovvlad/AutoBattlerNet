using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Gameplay.Character.Ui
{
    public class CharacterDamageFlasher : MonoBehaviour
    {
        [SerializeField] private Image _overlay;
        [SerializeField] private float _maxAlpha = 0.5f;
        [SerializeField] private float _fadeDuration = 0.4f;

        private Coroutine _coroutine;

        public void Flash(float intensity = 1f)
        {
            if (_overlay == null)
            {
                return;
            }

            var target = Mathf.Clamp01(intensity) * _maxAlpha;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(FlashRoutine(target));
        }

        private IEnumerator FlashRoutine(float targetAlpha)
        {
            var overlayColor = _overlay.color;
            overlayColor.a = targetAlpha;
            _overlay.color = overlayColor;

            var t = 0f;
            while (t < _fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                var alpha = Mathf.Lerp(targetAlpha, 0f, t / _fadeDuration);
                overlayColor.a = alpha;
                _overlay.color = overlayColor;
                yield return null;
            }

            overlayColor.a = 0f;
            _overlay.color = overlayColor;
            _coroutine = null;
        }

        private void Reset() =>
            _overlay = GetComponent<Image>();
    }
}