using System.Collections.Generic;
using Game.Gear.Common;

namespace Game.Block.Blocks.Gear
{
    public class GearComponent : IGear
    {
        public float CurrentRpm { get; }
        public float CurrentTorque { get; }
        public bool IsCurrentClockwise { get; }
        
        
        public bool IsDestroy { get; }
        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
        public int EntityId { get; }
        public IReadOnlyList<IGearEnergyTransformer> ConnectingTransformers { get; }
        public void SupplyRotation(float rpm, bool isClockwise)
        {
            throw new System.NotImplementedException();
        }
        public int TeethCount { get; }
    }
}