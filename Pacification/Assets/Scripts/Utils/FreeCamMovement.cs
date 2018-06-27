using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamMovement : MonoBehaviour {

    Camera mycam;
    float speed = 5;

    void Start()
    {
        mycam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        float sensitivity = 0.05f;
        float left = 0;
        float right = 0;
        float high = 0;

        Vector3 vp = mycam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mycam.nearClipPlane));
        vp.x -= 0.5f;
        vp.y -= 0.5f;
        vp.x *= sensitivity;
        vp.y *= sensitivity;
        vp.x += 0.5f;
        vp.y += 0.5f;
        Vector3 sp = mycam.ViewportToScreenPoint(vp);
        Vector3 v = mycam.ScreenToWorldPoint(sp);
        transform.LookAt(v, Vector3.up);

        if(Input.GetKey(KeyCode.Keypad6))
            transform.Translate(0.1f*speed, 0, 0);
        else if(Input.GetKey(KeyCode.Keypad4))
            transform.Translate(-0.1f * speed, 0, 0);

        if(Input.GetKey(KeyCode.Keypad8))
            transform.Translate(0, 0, 0.1f * speed);
        else if(Input.GetKey(KeyCode.Keypad2))
            transform.Translate(0, 0, -0.1f * speed);

        if(Input.GetKey(KeyCode.KeypadPlus))
            transform.Translate(0, 0.5f, 0);
        else if(Input.GetKey(KeyCode.KeypadMinus))
            transform.Translate(0, -0.5f, 0);

        if(Input.GetKey(KeyCode.Keypad9))
            ++speed;
        else if(Input.GetKey(KeyCode.Keypad3))
        {
            --speed;
            if(speed < 0)
                speed = 0;
        }
    }
}
