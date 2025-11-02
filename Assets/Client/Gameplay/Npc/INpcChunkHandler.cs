using Client.Gameplay.Map;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Npc
{
    [ContainableServiceContract("5544f085-9cb5-412e-872e-4c8266131d4f")]
    public interface INpcChunkHandler
    {
        ChunkGrid ChunkGrid { get; }
    }
}