public class TradeMessage : ChatMessage
{
    private static MessageType Type = MessageType.TRADE;
    public TradeMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#f5a5ea>" + _user + ": " + _message + "</color>";
    }
}