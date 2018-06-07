using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitsAnimator : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }
}
