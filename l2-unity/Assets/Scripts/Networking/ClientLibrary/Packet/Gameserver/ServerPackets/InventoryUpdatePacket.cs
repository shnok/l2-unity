public class InventoryUpdatePacket : AbstractItemPacket
{
    private ItemInstance[] _items;
    public ItemInstance[] Items { get { return _items; } }

    public InventoryUpdatePacket(byte[] d) : base(d){
        Parse();
    }

    public override void Parse() {
        int itemListSize = ReadI();

        _items = new ItemInstance[itemListSize];
        for(int i = 0; i < itemListSize; i++) {
            int lastChange = ReadI();
            _items[i] = ReadItem();
            _items[i].LastChange = lastChange;
        }
    }
}