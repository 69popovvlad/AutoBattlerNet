using Client.Gameplay.Npc.Network;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Npc
{
    [ContainableServiceContract("f9bc6acc-c611-42af-8a60-f64739272832")]
    public interface INpcGhostContainer
    {
        bool TryGetGhost(uint entityId, out NpcGhost ghost);
    }
}