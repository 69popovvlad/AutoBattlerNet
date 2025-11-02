using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Network.Character
{
    [ContainableServiceContract("ec9988f6-a8ad-4e17-b6ed-61aeb0f73745")]
    public interface ICharacterContainer
    {
        bool TryGetRandom(out CharacterContext ctx);

        bool TryAdd(CharacterContext context);

        bool Remove(CharacterContext context);
    }
}