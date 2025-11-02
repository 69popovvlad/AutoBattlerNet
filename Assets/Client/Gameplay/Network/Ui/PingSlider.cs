using FishNet.Managing;
using FishNet.Managing.Transporting;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Gameplay.Network.Ui
{
    public class PingSlider : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _label;

        private TransportManager _tm;

        private void Awake()
        {
            _tm = _networkManager.TransportManager;

            var ls = _tm.LatencySimulator;
            ls.SetEnabled(true);

            _slider.minValue = 0f;
            _slider.maxValue = 1000f;
            _slider.wholeNumbers = true;
            _slider.onValueChanged.AddListener(SetLatency);

            SetLatency(_slider.value);
        }

        private void SetLatency(float ms)
        {
            var ls = _tm.LatencySimulator;
            ls.SetLatency(Mathf.RoundToInt(ms));

            if (_label)
            {
                _label.text = $"{ls.GetLatency()} ms";
            }
        }
    }
}