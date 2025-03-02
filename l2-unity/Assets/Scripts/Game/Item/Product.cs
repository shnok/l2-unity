public class Product
{
    public Product() { }
    public Product(Product old)
    {
        Type1 = old.Type1;
        ObjectId = old.ObjectId;
        ItemId = old.ItemId;
        Count = old.Count;
        Price = old.Price;
        Type2 = old.Type2;
        BodyPart = old.BodyPart;
    }

    public ItemType1 Type1 { get; set; }
    public int ObjectId { get; set; }
    public int ItemId { get; set; }
    public int Count { get; set; }
    public int Price { get; set; }
    public ItemType2 Type2 { get; set; }
    public ItemSlot BodyPart { get; set; }
}