using UnityEngine;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Camera
{
    [ContainableServiceImplementation("ff46c654-ff75-478d-840c-ce9ad63d7084", "5f9321fb-802e-4a33-9682-010bd59e54d8")]
    public class CameraTargetProvider : ICameraTargetProvider
    {
        public Transform CameraTarget { get; set; }
    }
}