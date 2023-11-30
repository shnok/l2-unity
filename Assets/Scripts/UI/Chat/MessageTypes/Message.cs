public abstract class Message {
    public MessageType messageType;

    public Message(MessageType messageType) {
        this.messageType = messageType;
    }

    public abstract override string ToString();
}