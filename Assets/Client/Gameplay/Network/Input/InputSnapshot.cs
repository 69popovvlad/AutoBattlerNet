using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public struct InputSnapshot
    {
        public uint Sequence;
        public Vector2 Direction;
        public byte Flags;
    }
}