using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBehaviour : MonoBehaviour
{
    //private static x = new PhysicalConst();
    public float temperature;
    private double velocity;
    public const float mass=32f;
    //Random rnd = new static Random();
    private const double sigma=1f/*=PhysicalConst.BOLTZMANN*/; //standard deviation
    private const double mi=1f; //mean

    private void SetVelocityStd()
    {
        //Box-Muller Transform
        //var rnd = new Random();
        //Random rnd = new Random();
        var u1=1f;
        var u2=4f;
        var z=Mathf.Sqrt(-2*Mathf.Log(u1))*Mathf.Cos(2*Mathf.PI*u2);
        velocity = z*sigma+mi;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }
}
