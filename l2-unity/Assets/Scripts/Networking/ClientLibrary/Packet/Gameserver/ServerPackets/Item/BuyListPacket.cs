using System;
using UnityEngine;

public class BuyListPacket : ServerPacket
{
    public int Adena { get; private set; }
    public int ListId { get; private set; }
    public int ListSize { get; private set; }

    public BuyListPacket(byte[] d) : base(d)
    {
        Parse();
    }

    public override void Parse()
    {
        Adena = ReadI();
        ListId = ReadI();

        int count = ReadH();

        Product[] products = new Product[count];
        for (int i = 0; i < count; i++)
        {
            products[i] = new Product();
            products[i].Type1 = (ItemType1)ReadH();
            products[i].ItemId = ReadI();
            products[i].Count = ReadI();
            products[i].Type2 = (ItemType2)ReadH();
            ReadH();
            products[i].BodyPart = (ItemSlot)ReadI();
            ReadH();
            ReadH();
            ReadH();
            products[i].Price = ReadI();
        }
    }
}