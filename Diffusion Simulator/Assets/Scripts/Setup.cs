using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 50;
    public GameObject Particle;
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0;i<NUM_PARTICLES;i++)
            Instantiate(Particle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}