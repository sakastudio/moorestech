using Game.Block.Interface;
using Game.World.Interface.DataStore;

namespace Game.World.Interface.Event
{
    public class BlockRemoveEventProperties
    {
        public readonly IBlock Block;
        public readonly Coordinate Coordinate;

        public BlockRemoveEventProperties(Coordinate coordinate, IBlock block)
        {
            Coordinate = coordinate;
            Block = block;
        }
    }
}