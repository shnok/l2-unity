using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    [SerializeField] private ObjectData _surfaceObject;

    public string GetSurfaceTag()
    {
        if (World.Instance == null)
        {
            return "stone";
        }

        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out var hit, 2f, World.Instance.GroundMask))
        {
            _surfaceObject = new ObjectData(hit.collider.gameObject);
        }
        else
        {
            return "Dirt";
        }

        return _surfaceObject.ObjectTag;
    }
}
