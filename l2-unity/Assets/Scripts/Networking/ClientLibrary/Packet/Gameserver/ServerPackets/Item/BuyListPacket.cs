using System;
using UnityEngine;

public class BuyListPacket : ServerPacket
{
    public int Adena { get; private set; }
    public int ListId { get; private set; }
    public bool OpenTab { get; private set; }
    public Product[] Products { get; private set; }

    public BuyListPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        OpenTab = ReadB() == 1;
        Adena = ReadI();
        ListId = ReadI();
        int count = ReadH();

        Products = new Product[count];
        for (int i = 0; i < count; i++)
        {
            Products[i] = new Product();
            Products[i].Type1 = (ItemType1)ReadH();
            Products[i].ItemId = ReadI();
            ReadI();
            Products[i].Count = ReadI();
            Products[i].Type2 = (ItemType2)ReadH();
            ReadH();
            Products[i].BodyPart = (ItemSlot)ReadI();
            ReadH();
            ReadH();
            ReadH();
            Products[i].Price = ReadI();
        }
    }
}