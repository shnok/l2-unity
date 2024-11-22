public class ClanMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.CLAN;
    public ClanMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#7D77FF>" + _user + ": " + _message + "</color>";
    }
}