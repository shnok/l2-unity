public class CriticalAnnounceMesasge : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.CRITICAL_ANNOUNCE;
#pragma warning restore 0414
    public CriticalAnnounceMesasge(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#00FFFF>" + _user + ": " + _message + "</color>";
    }
}