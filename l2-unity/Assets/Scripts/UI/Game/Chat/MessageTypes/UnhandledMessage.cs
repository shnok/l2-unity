public class UnhandledMessage : SystemMessage {
    public UnhandledMessage() : base(null, null) {
    }

    public override string ToString() {
        return "<color=#799BB0FF>Unknown system message.</color>";
    }
}
