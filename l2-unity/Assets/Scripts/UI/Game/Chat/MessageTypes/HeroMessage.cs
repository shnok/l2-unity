public class HeroMessage : ChatMessage
{
    private static L2MessageType Type = L2MessageType.HERO_VOICE;
    public HeroMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#408CFF>" + _user + ": " + _message + "</color>";
    }
}