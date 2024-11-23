using UnityEngine;
using System.Collections;



namespace AdvancedTerrainGrass.Demo {
 
	public class ExtendedFlycam : MonoBehaviour
	{


	// slightly changed....
	 
		/*
		EXTENDED FLYCAM
			Desi Quintans (CowfaceGames.com), 17 August 2012.
			Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
	 
		LICENSE
			Free as in speech, and free as in beer.
	 
		FEATURES
			WASD/Arrows:    Movement
			          Q:    Dropp
			          E:    Climb
	                      Shift:    Move faster
	                    Control:    Move slower
	                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
		*/
	 
		public float cameraSensitivity = 90;
		public float climbSpeed = 4;
		public float normalMoveSpeed = 10;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3;
	 
		private float rotationX = 0.0f;
		private float rotationY = 0.0f;
	 
		void Start () {
			rotationX = transform.eulerAngles.y;
		}

	 	void Update ()
		{
			// Cache deltaTime!
			var deltaTime = Time.deltaTime;	
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * deltaTime;
			rotationY = Mathf.Clamp (rotationY, -90, 90);
	 
			var tempRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			tempRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, tempRotation, deltaTime * 6.0f);
	 
		 	if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
		 	{
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * deltaTime;
		 	}
		 	else if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl))
		 	{
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * deltaTime;
		 	}
		 	else
		 	{
				transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * deltaTime;
				transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * deltaTime;
		 	}
	 
			if (Input.GetKey (KeyCode.Q)) {transform.position -= transform.up * climbSpeed * deltaTime;}
			if (Input.GetKey (KeyCode.E)) {transform.position += transform.up * climbSpeed * deltaTime;}
		}
	}

}