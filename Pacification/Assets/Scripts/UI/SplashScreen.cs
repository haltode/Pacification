using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

    private float f;
    private void Start()
    {
        f = 0f;
    }
    void Update()
    {
        if (f > 2.5f || Input.anyKey)
            SceneManager.LoadScene("Menu");
        else
            f += Time.deltaTime;
    }
}
