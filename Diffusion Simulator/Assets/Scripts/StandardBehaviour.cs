using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardBehaviour : MonoBehaviour
{
    //private static x = new PhysicalConst();
    public float temperature;
    private double velocity;
    public const float mass=32f;
    //Random rnd = new static Random();
    private double sigma; //standard deviation
    private double mi=0f; //mean

    private void SetVelocityStd()
    {
        //Box-Muller Transform
        var u1=UnityEngine.Random.Range(0.001f,1);
        var u2=UnityEngine.Random.Range(0.001f,1);
        var z=Mathf.Sqrt(-2*Mathf.Log(u1))*Mathf.Cos(2*Mathf.PI*u2);
        velocity = z*sigma+mi;
        Debug.Log(velocity);
    }

    // Start is called before the first frame update
    void Start()
    {
        sigma = Math.Sqrt(PhysicalConst.BOLTZMANN*temperature/mass);
        //sigma*=1e+11;
        //sigma=1f;
    }

    // Update is called once per frame
    void Update()
    {
        SetVelocityStd();
    }

    void FixedUpdate()
    {

    }
}
