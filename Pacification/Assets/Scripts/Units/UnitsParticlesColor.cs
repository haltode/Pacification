using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsParticlesColor : MonoBehaviour
{
    private ParticleSystem ps;

    public void SetColor(Color color)
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
    }
}
