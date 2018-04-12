using UnityEngine;

public class LUI_PressAnyKey : MonoBehaviour {

	[Header("OBJECTS")]
	public GameObject scriptObject;
	public Animator animatorComponent;

	void Start ()
	{
		animatorComponent.GetComponent<Animator>();
	}

	void Update ()
	{
		if (Input.anyKeyDown) 
		{
			animatorComponent.Play ("Bloody Splash Anim");
			Destroy (scriptObject);
		}
	}
}