public class MessageLoggedIn : SystemMessage {
    private string _username;

    public MessageLoggedIn(string username) {
        _username = username;
    }

    public override string ToString() {
        return "<color=#7EF9F9>Player " + _username + " logged in.</color>";
    }
}