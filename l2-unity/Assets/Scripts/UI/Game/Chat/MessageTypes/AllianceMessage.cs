public class AllianceMessage : ChatMessage
{
    private static MessageType Type = MessageType.ALLIANCE;

    public AllianceMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#77FF99>" + _user + ": " + _message + "</color>";
    }
}