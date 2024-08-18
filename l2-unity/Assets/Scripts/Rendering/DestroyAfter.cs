using System.Collections;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] private int _destroyAfterMs = 200;
    [SerializeField] private bool _disableOnly;

    void Start() {
        StartCoroutine(DestroyAfterMs());
    }

    IEnumerator DestroyAfterMs() {
        yield return new WaitForSeconds(_destroyAfterMs / 1000f);
        if(_disableOnly) {
            gameObject.SetActive(false);
        } else {
            Destroy(this.gameObject);
        }
    }
}
