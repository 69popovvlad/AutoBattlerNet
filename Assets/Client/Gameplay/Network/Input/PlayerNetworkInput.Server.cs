using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace Client.Gameplay.Network.Input
{
    public partial class PlayerNetworkInput
    {
        private uint _lastConfirmedSeqServer;

        [ServerRpc]
        // ReSharper disable once SuggestBaseTypeForParameter;
        // Cause FishNet can't serialize IReadOnlyList<InputSnapshot>
        private void SubmitInputServerRpc(InputSnapshot[] batch, NetworkConnection sender = null)
        {
            if (batch == null || batch.Length == 0)
            {
                return;
            }

            var anyApplied = false;
            PlayerState lastState = default;
            for (var i = 0; i < batch.Length; ++i)
            {
                var inputSnapshot = batch[i];
                if (!SeqGreater(inputSnapshot.Sequence, _lastConfirmedSeqServer))
                {
                    continue;
                }

                // Server prediction
                _rider.ApplyInputStep(inputSnapshot.Direction, SIM_DT);

                var kinematicState = _rider.GetState();
                lastState = new PlayerState
                {
                    LastSequence = inputSnapshot.Sequence,
                    Position = kinematicState.Position,
                    Velocity = kinematicState.Velocity,
                    Yaw = kinematicState.Yaw
                };

                _lastConfirmedSeqServer = inputSnapshot.Sequence;
                anyApplied = true;
            }

            if (!anyApplied)
            {
                return;
            }

            AckTargetRpc(sender, lastState);
            StateForObserversRpc(lastState);
        }


        [ObserversRpc(ExcludeOwner = true)]
        private void StateForObserversRpc(PlayerState playerState)
        {
            // For not owners, but clients set current state
            var kinematicState = _rider.GetState();

            // Simple lerp
            kinematicState.Position = Vector3.Lerp(kinematicState.Position, playerState.Position, 0.8f);
            kinematicState.Velocity = Vector3.Lerp(kinematicState.Velocity, playerState.Velocity, 0.8f);
            kinematicState.Yaw = Mathf.LerpAngle(kinematicState.Yaw, playerState.Yaw, 0.8f);

            _rider.SetState(kinematicState);
        }

        [System.Runtime.CompilerServices.MethodImpl(
            System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static bool SeqGreater(uint a, uint b) => unchecked((int)(a - b) > 0);
    }
}