using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 500;
    public GameObject Particle;
    private StandardBehaviour particleScript;
    public float temperature;
    public string particleType;
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0;i<NUM_PARTICLES;i++)
        {
            Instantiate(Particle);
            particleScript = Particle.GetComponent<StandardBehaviour>();
            particleScript.temperature=temperature;
            particleScript.particleType=ParticleType.CreateFromJSON(particleType);
            var x = new ParticleJSONSerializer();
            x = ParticleJSONSerializer.CreateFromJSON("Oxygen");
            x.ToStringDebug();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}