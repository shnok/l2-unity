using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkAnimationReceive : BaseAnimationController {
    protected override void Initialize() {
        if (World.Instance.OfflineMode) {
            this.enabled = false;
            return;
        }

        base.Initialize();
    }
}
