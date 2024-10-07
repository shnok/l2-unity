using System.Collections;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SkinnedMeshSync : MonoBehaviour
{
    [SerializeField] private Transform _bodyPartsContainer;
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;
    [SerializeField] private Transform[] _bones;

    void Start()
    {
        SyncMesh();
    }

    void Update()
    {
#if (UNITY_EDITOR)
        if (!EditorApplication.isPlaying)
        {
            SyncMesh();
        }
#endif
    }

    public void SyncMesh()
    {
        // Only refresh once the entity is enabled
        if (this.gameObject.activeSelf && this.gameObject.activeInHierarchy)
        {
            StartCoroutine(SyncTask());
        }
    }

    private IEnumerator SyncTask()
    {
        if (_bodyPartsContainer == null)
        {
            _bodyPartsContainer = transform.parent.GetChild(1);
        }
        if (_rootSkinnedRenderer == null)
        {
            _rootSkinnedRenderer = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        }

        float startTime = Time.time;
        while (_bodyPartsContainer.childCount > 7)
        {
            yield return new WaitForEndOfFrame();
            if (Time.time - startTime < 1.0f)
            {
                Debug.LogWarning("Could not sync mesh.");
                yield break;
            }
        }

        DoSync();
    }

    private void DoSync()
    {
        // Debug.LogWarning($"[{transform.name}] SyncMesh");
        _bodyPartsContainer.gameObject.SetActive(true);

        // Retrieving SkinnedMeshRenderers
        _destSkinnedRenderer = new SkinnedMeshRenderer[8];

        for (byte i = 0; i < _bodyPartsContainer.childCount; i++)
        {
            Transform child = _bodyPartsContainer.GetChild(i);
            _destSkinnedRenderer[i] = child.GetComponent<SkinnedMeshRenderer>();
        }

        // Updating body parts bones and bounds
        Bounds bounds = new Bounds();
        bounds.center = new Vector3(0, 0, 0f);
        bounds.extents = new Vector3(0.0025f, 0.002f, 0.004f);
        bounds.size = bounds.extents * 2f;
        foreach (var renderer in _destSkinnedRenderer)
        {
            if (renderer != null)
            {
                _bones = _rootSkinnedRenderer.bones;
                renderer.rootBone = _rootSkinnedRenderer.rootBone;
                renderer.transform.localScale = Vector3.one;
                renderer.bones = _rootSkinnedRenderer.bones;
                renderer.localBounds = bounds;
            }
        }
    }
}
