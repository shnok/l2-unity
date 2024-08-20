using System.Collections;
using UnityEngine;

public class SkinnedMeshSync : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;

    void Start()
    {
        SyncMesh();
    }

    public void SyncMesh()
    {
        if (this.gameObject.activeSelf && this.gameObject.activeInHierarchy)
        {
            StartCoroutine(SyncTask());
        }
    }

    private IEnumerator SyncTask()
    {
        float startTime = Time.time;
        while (transform.parent.childCount > 8)
        {
            yield return new WaitForEndOfFrame();
            if (Time.time - startTime < 1.0f)
            {
                Debug.LogWarning("Could not sync mesh.");
                yield break;
            }
        }

        _destSkinnedRenderer = new SkinnedMeshRenderer[8];
        byte childIndex = 0;

        for (byte i = 0; i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);
            if (child != transform)
            {
                //  Debug.Log(childIndex + " : " + child);
                _destSkinnedRenderer[childIndex++] = child.GetComponentInChildren<SkinnedMeshRenderer>();
            }
            else
            {
                _rootSkinnedRenderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        foreach (var renderer in _destSkinnedRenderer)
        {
            if (renderer != null)
            {
                renderer.bones = _rootSkinnedRenderer.bones;
            }
            //renderer.rootBone = _rootSkinnedRenderer.rootBone;
        }
    }
}
