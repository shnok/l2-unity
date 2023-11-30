public abstract class SystemMessage : Message {
    protected SystemMessage() : base(MessageType.SystemMessage) {
    }

    public abstract override string ToString();
}