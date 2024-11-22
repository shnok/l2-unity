public class CriticalAnnounceMesasge : ChatMessage
{
    private static L2MessageType Type = L2MessageType.CRITICAL_ANNOUNCE;
    public CriticalAnnounceMesasge(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#00FFFF>" + _user + ": " + _message + "</color>";
    }
}