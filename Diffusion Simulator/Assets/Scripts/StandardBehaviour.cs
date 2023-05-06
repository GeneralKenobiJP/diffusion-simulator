using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardBehaviour : MonoBehaviour
{
    //private static x = new PhysicalConst();
    public float temperature;
    private float velocity;
    private Vector3 direction = new Vector3();
    private double mass;
    private float molarMass=0.032f;
    private double sigma; //standard deviation
    private double mi; //mean

    private void SetVelocityStd()
    {
        //Box-Muller Transform
        var u1=UnityEngine.Random.Range(0.001f,1);
        var u2=UnityEngine.Random.Range(0.001f,1);
        var z=Mathf.Sqrt(-2*Mathf.Log(u1))*Mathf.Cos(2*Mathf.PI*u2);
        velocity = (float)(z*sigma+mi);
        Debug.Log(velocity);
        //
        //cannot do Maxwell-Boltzmann Distribution of Speeds implementation because no, so approximating with Gaussian
        //Debug.Log(velocity);
    }
    private void GenerateDirection()
    {
        direction.Set(UnityEngine.Random.Range(1f,10f),UnityEngine.Random.Range(1f,10f),UnityEngine.Random.Range(1f,10f));
        direction.Normalize();
        var relativeVelocity = 0.2f*velocity/(float)mi;
        direction.Set(direction.x*relativeVelocity,this.direction.y*relativeVelocity,this.direction.z*relativeVelocity);
        Debug.Log(direction);
        Debug.Log(direction.magnitude);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position=new Vector3(0f, 3.2f, -7.53f);
        mass=molarMass*PhysicalConst.uToKg;
        mi=Math.Sqrt(2*PhysicalConst.GAS_CONSTANT*temperature/molarMass);
        //mi=0f;
        sigma = Math.Sqrt(PhysicalConst.BOLTZMANN*temperature/molarMass);
        sigma*=5e+11; //making sigma more sensible for the Gaussian approach
        Debug.Log(sigma);
        Debug.Log(mi);
        SetVelocityStd();
        GenerateDirection();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(direction*Time.deltaTime);
    }

    void FixedUpdate()
    {

    }
}
