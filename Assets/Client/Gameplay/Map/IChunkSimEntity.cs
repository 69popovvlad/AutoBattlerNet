using UnityEngine;

namespace Client.Gameplay.Map
{
    /// Minimal contract for any entity simulated in ChunkSimManager
    public interface IChunkSimEntity
    {
        uint Id { get; }

        bool IsActive { get; }

        Vector3 Position { get; }

        void Simulate(float delta);
    }
}