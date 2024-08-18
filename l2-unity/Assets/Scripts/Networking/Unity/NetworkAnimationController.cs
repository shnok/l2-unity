public class NetworkAnimationController : BaseAnimationController {
    public override void Initialize() {
        if (World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }

        base.Initialize();
    }
}
