using System.Collections.Generic;
using Client.Gameplay.Character.Network;
using Validosik.Core.Ioc.Attributes;

namespace Client.Gameplay.Character
{
    [ContainableServiceImplementation("ec9988f6-a8ad-4e17-b6ed-61aeb0f73745", "9982c958-2085-4142-b4ed-7f5b38dcedc8")]
    public class CharacterContainer : ICharacterContainer
    {
        private readonly Dictionary<int, CharacterContext> _characters = new Dictionary<int, CharacterContext>();

        public bool TryGetRandom(out CharacterContext context)
        {
            if (_characters.Count == 0)
            {
                context = null;
                return false;
            }

            var index = UnityEngine.Random.Range(0, _characters.Count);
            foreach (var c in _characters.Values)
            {
                if (index-- != 0)
                {
                    continue;
                }

                context = c;
                return true;
            }

            context = null;
            return false;
        }

        public bool TryGet(int objectId, out CharacterContext context) =>
            _characters.TryGetValue(objectId, out context);

        public bool TryAdd(CharacterContext context) =>
            _characters.TryAdd(context.NetworkObject.ObjectId, context);

        public bool Remove(CharacterContext context) =>
            _characters.Remove(context.NetworkObject.ObjectId);
    }
}