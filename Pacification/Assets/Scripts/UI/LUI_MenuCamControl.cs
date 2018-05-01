using UnityEngine;
using System.Collections;

public class LUI_MenuCamControl : MonoBehaviour {

	[Header("OBJECTS")]
	public Transform currentMount;
	public new Camera camera;

	[Header("SETTINGS")]
	[Tooltip("Set 1.1 for instant fly")]
	[Range(0.01f,1.1f)]public float speed = 0.1f;
	public float zoom = 1.0f;

	void Update ()
	{
		transform.position = Vector3.Lerp(transform.position,currentMount.position,speed);
		transform.rotation = Quaternion.Slerp(transform.rotation,currentMount.rotation,speed);
		camera.fieldOfView = 60 + zoom;
	}

	public void setMount (Transform newMount)
	{
		currentMount = newMount;
	}
}