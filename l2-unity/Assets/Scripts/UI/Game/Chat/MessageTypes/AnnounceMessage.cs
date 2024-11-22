public class AnnounceMesasge : ChatMessage
{
    private static L2MessageType Type = L2MessageType.ANNOUNCEMENT;
    public AnnounceMesasge(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#80FFFF>" + _user + ": " + _message + "</color>";
    }
}