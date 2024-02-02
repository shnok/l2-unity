public abstract class Message {
    private MessageType _messageType;
    public MessageType MessageType { get { return _messageType; } }

    public Message(MessageType messageType) {
        _messageType = messageType;
    }

    public abstract override string ToString();
}