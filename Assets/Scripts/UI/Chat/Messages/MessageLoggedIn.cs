public class MessageLoggedIn : SystemMessage {
    private string _username;
    
    public MessageLoggedIn(string username) {
        _username = username;
    }
    
    public override string ToString() {
        return "<color=#5AB0B2>Player " + _username + " logged in.</color>";
    } 
}