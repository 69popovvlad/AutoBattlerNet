using UnityEngine;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Camera
{
    [ContainableServiceContract("ff46c654-ff75-478d-840c-ce9ad63d7084")]
    public interface ICameraTargetProvider
    {
        Transform CameraTarget { get; set; }
    }
}