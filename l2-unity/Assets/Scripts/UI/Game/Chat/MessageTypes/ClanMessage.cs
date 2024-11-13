public class ClanMessage : ChatMessage
{
    private static MessageType Type = MessageType.CLAN;
    public ClanMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#7D77FF>" + _user + ": " + _message + "</color>";
    }
}