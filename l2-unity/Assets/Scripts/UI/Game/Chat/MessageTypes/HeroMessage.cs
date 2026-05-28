public class HeroMessage : ChatMessage
{
#pragma warning disable 0414
    private static L2MessageType Type = L2MessageType.HERO_VOICE;
#pragma warning restore 0414
    public HeroMessage(string user, string message) : base(user, message)
    {
    }

    public override string ToString()
    {
        return "<color=#408CFF>" + _user + ": " + _message + "</color>";
    }
}