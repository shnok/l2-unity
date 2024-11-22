public class AllianceMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.ALLIANCE;

    public AllianceMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#77FF99>" + _user + ": " + _message + "</color>";
    }
}