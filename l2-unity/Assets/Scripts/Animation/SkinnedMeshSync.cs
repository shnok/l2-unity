using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class SkinnedMeshSync : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;
    [SerializeField] private Transform _bodyPartsContainer;

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
        if (_bodyPartsContainer == null)
        {
            _bodyPartsContainer = transform.GetChild(2);
        }
        if (_rootSkinnedRenderer == null)
        {
            _rootSkinnedRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        }

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

        // Retrieving SkinnedMeshRenderers
        _destSkinnedRenderer = new SkinnedMeshRenderer[8];

        for (byte i = 0; i < _bodyPartsContainer.childCount; i++)
        {
            Transform child = _bodyPartsContainer.GetChild(i);
            _destSkinnedRenderer[i] = child.GetComponent<SkinnedMeshRenderer>();
        }

        // Updating body parts bones and bounds
        Bounds bounds = new Bounds();
        bounds.center = new Vector3(0, 0, 0.5f);
        bounds.extents = new Vector3(0.25f, 0.2f, 0.4f);
        bounds.size = new Vector3(0.5f, 0.4f, 0.8f);
        foreach (var renderer in _destSkinnedRenderer)
        {
            if (renderer != null)
            {
                renderer.transform.localScale = Vector3.one;
                renderer.bones = _rootSkinnedRenderer.bones;
                renderer.localBounds = bounds;
            }
        }
    }
}
