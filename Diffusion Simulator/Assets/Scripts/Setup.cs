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
    /*public ParticleType x;
    public ParticleType y;
    public ParticleType z;*/
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0;i<NUM_PARTICLES;i++)
        {
            Instantiate(Particle);
            particleScript = Particle.GetComponent<StandardBehaviour>();
            particleScript.temperature=temperature;
            //particleScript.particleType=ParticleType.CreateFromJSON(particleType);
            //var x = new ParticleJSONSerializer();
            //x = ParticleJSONSerializer.CreateFromJSON("Oxygen");
            //x.ToStringDebug();
        }
        /*x = new ParticleType("Oxygen");
            x.boilingPoint = "150";
            x.molarMass = "30";
            x.bondType = "ionic";
            y = new ParticleType("Hydrogen");
            x.boilingPoint = "100";
            x.molarMass = "2";
            x.bondType = "nonpolar covalent";
            z = new ParticleType("Carbon monoxide");
            x.boilingPoint = "600";
            x.molarMass = "26";
            x.bondType = "polar covalent";
            var alpha = new ParticleJSONSerializer();
            x.ToStringDebug();
            y.ToStringDebug();
            z.ToStringDebug();
            alpha._particleTypes.Add(x);
            alpha._particleTypes.Add(y);
            alpha._particleTypes.Add(z);
            ParticleJSONSerializer.SaveIntoJSON(alpha);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}