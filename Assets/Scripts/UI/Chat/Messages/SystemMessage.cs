public abstract class SystemMessage {
    private byte[] _data;

    public SystemMessage() {}
    public SystemMessage(byte[] data) {
        _data = data;
    }
    
    public abstract override string ToString(); 
}