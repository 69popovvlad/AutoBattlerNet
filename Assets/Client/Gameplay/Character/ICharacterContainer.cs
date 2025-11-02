using Client.Gameplay.Network.Character;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Character
{
    [ContainableServiceContract("ec9988f6-a8ad-4e17-b6ed-61aeb0f73745")]
    public interface ICharacterContainer
    {
        bool TryGetRandom(out CharacterContext context);
        
        bool TryGet(int objectId, out CharacterContext context);

        bool TryAdd(CharacterContext context);

        bool Remove(CharacterContext context);
    }
}