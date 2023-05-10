using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 500;
    public GameObject Particle;
    public float temperature;
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0;i<NUM_PARTICLES;i++)
        {
            Instantiate(Particle);
            Particle.GetComponent<StandardBehaviour>().temperature=temperature;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}