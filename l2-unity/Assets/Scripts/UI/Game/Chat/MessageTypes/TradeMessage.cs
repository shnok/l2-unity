public class TradeMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.TRADE;
#pragma warning restore 0414
    public TradeMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#f5a5ea>" + _user + ": " + _message + "</color>";
    }
}