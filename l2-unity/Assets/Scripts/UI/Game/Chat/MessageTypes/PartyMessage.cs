public class PartyMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.PARTY;
    public PartyMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        // return "<color=#4AD519>" + _user + ": " + _message + "</color>";
        return "<color=#00FF00>" + _user + ": " + _message + "</color>";
    }
}