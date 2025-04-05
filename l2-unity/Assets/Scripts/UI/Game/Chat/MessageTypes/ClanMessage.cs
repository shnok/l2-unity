public class ClanMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.CLAN;
#pragma warning restore 0414
    public ClanMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#7D77FF>" + _user + ": " + _message + "</color>";
    }
}