public class RequestCharCreatePacket : ClientPacket
{
    public RequestCharCreatePacket(string name, CharacterRace race, CharacterSex sex, CharacterClass characterClass, int hairstyle, int haircolor, int face) : base((byte)GameClientPacketType.RequestCharCreate)
    {
        WriteS(name);
        WriteI((int)race);
        WriteI((int)sex);
        WriteI((int)characterClass);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteI(0);
        WriteI(hairstyle);
        WriteI(haircolor);
        WriteI(face);
        BuildPacket();
    }
}
