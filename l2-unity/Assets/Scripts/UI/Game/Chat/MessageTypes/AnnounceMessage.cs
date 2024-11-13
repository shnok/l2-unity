public class AnnounceMesasge : ChatMessage
{
    private static MessageType Type = MessageType.ANNOUNCEMENT;
    public AnnounceMesasge(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#80FFFF>" + _user + ": " + _message + "</color>";
    }
}