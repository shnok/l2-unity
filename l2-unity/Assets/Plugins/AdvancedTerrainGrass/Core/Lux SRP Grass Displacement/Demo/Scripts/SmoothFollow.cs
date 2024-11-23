using UnityEngine;

namespace Lux_SRP_GrassDisplacement
{

	public class SmoothFollow : MonoBehaviour {
	    public Transform targetTransform;
	    public float smoothTime = 0.15F;
	    private Vector3 velocity = Vector3.zero;
	    
	    void Update() {
	        Vector3 targetPosition = targetTransform.position;
	        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
	    }
	}
}