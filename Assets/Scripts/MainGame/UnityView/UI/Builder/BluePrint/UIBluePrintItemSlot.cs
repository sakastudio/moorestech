namespace MainGame.UnityView.UI.Builder.BluePrint
{
    public class UIBluePrintItemSlot : ISubInventoryElement
    {
        public UIBluePrintType ElementType => UIBluePrintType.OneSlot;
        public int Priority { get; }

        //表示するX座標
        public readonly float X;
        //表示するY座標
        public readonly float Y;
        //表示する順番

        public readonly InventorySlotElementOptions Options;

        public UIBluePrintItemSlot(float x, float y, int priority, InventorySlotElementOptions options)
        {
            X = x;
            Y = y;
            Priority = priority;
            Options = options;
        }
    }
}