public class HeroMessage : ChatMessage
{
    private static MessageType Type = MessageType.HERO_VOICE;
    public HeroMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#408CFF>" + _user + ": " + _message + "</color>";
    }
}