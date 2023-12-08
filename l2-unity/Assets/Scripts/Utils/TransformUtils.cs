using System;
using UnityEngine;

public static class TransformUtils {
    public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

    public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector) {
        foreach(Transform child in self) {
            if(selector(child)) {
                return child;
            }

            var finding = child.FindRecursive(selector);

            if(finding != null) {
                return finding;
            }
        }

        return null;
    }
}