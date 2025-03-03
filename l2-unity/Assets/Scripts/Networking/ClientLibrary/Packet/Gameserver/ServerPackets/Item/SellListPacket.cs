using System;
using UnityEngine;

public class SellListPacket : ServerPacket
{
    public int Adena { get; private set; }
    public bool OpenTab { get; private set; }
    public Product[] Products { get; private set; }

    public SellListPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        OpenTab = ReadB() == 1;
        Adena = ReadI();
        ReadI();

        int count = ReadH();

        Products = new Product[count];
        for (int i = 0; i < count; i++)
        {
            Products[i] = new Product();
            Products[i].Type1 = (ItemType1)ReadH();
            Products[i].ObjectId = ReadI();
            Products[i].ItemId = ReadI();
            Products[i].Count = ReadI();
            Products[i].Type2 = (ItemType2)ReadH();
            ReadH(); //custom type 1
            Products[i].BodyPart = (ItemSlot)ReadI();
            ReadH(); //enchant level
            ReadH(); //custom type 2
            ReadH();
            Products[i].Price = ReadI();
        }
    }
}