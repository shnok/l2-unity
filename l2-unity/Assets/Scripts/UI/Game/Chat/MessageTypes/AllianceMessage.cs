public class AllianceMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.ALLIANCE;
#pragma warning restore 0414

    public AllianceMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#77FF99>" + _user + ": " + _message + "</color>";
    }
}