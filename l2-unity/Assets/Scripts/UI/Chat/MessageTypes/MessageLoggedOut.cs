public class MessageLoggedOut : SystemMessage {
    private string _username;

    public MessageLoggedOut(string username) {
        _username = username;
    }

    public override string ToString() {
        return "<color=#7EF9F9>Player " + _username + " logged out.</color>";
    }
}