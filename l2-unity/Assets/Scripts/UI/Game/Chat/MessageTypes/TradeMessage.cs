public class TradeMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.TRADE;
    public TradeMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#f5a5ea>" + _user + ": " + _message + "</color>";
    }
}