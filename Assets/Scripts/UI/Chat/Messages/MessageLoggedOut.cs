public class MessageLoggedOut : SystemMessage {
    private string _username;
    
    public MessageLoggedOut(string username) {
        _username = username;
    }
    
    public override string ToString() {
        return "<color=#5AB0B2>Player " + _username + " logged out.</color>";
    } 
}