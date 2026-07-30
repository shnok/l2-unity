public class ChatMessage
{
    private static L2MessageType Type = L2MessageType.ALL;
    protected string _user;
    protected string _message;

    public L2MessageType MessageType { get => Type; }

    public ChatMessage(string user, string message)
    {
        _user = user;
        _message = message;
    }

    public override string ToString()
    {
        // return "<color=#DDDDDD>" + _user + ": " + _message + "</color>";
        return "<color=#B09B79>" + _user + ": " + _message + "</color>";
    }
}
