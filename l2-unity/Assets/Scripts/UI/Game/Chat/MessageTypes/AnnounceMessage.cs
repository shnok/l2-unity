public class AnnounceMesasge : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.ANNOUNCEMENT;
#pragma warning restore 0414
    public AnnounceMesasge(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#80FFFF>" + _user + ": " + _message + "</color>";
    }
}