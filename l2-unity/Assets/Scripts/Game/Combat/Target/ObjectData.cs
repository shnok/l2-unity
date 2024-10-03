using UnityEngine;

[System.Serializable]
public class ObjectData
{
    [SerializeField] private string _objectLayerName;
    [SerializeField] private int _objectLayer;
    [SerializeField] private string _objectTag;
    [SerializeField] private string _objectScene;
    [SerializeField] private Entity _objectEntity;
    [SerializeField] private Transform _objectTransform;

    public string ObjectLayerName { get { return _objectLayerName; } }
    public int ObjectLayer { get { return _objectLayer; } }
    public string ObjectTag { get { return _objectTag; } }
    public string ObjectScene { get { return _objectScene; } }
    public Transform ObjectTransform { get { return _objectTransform; } }
    public Entity Entity { get { return _objectEntity; } }

    public ObjectData(GameObject gameObject)
    {
        _objectTransform = gameObject.transform;
        _objectTag = gameObject.tag;
        _objectLayerName = LayerMask.LayerToName(gameObject.layer);
        _objectLayer = gameObject.layer;
        _objectScene = gameObject.scene.name;
        _objectEntity = gameObject.GetComponent<Entity>();
    }
}