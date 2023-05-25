using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 500;
    public GameObject Particle;
    private StandardBehaviour particleScript;
    public float temperature;
    private string particleType="chlorine"; //private as for now
    // Start is called before the first frame update
    void Start()
    {
        for(var i=0;i<NUM_PARTICLES;i++)
        {
            Instantiate(Particle);
            particleScript = Particle.GetComponent<StandardBehaviour>();
            particleScript.temperature=temperature;
            particleScript.particleType=jsonSerializer.SearchForParticle(particleType);
            /*Debug.Log(particleScript.particleType.boilingPoint);
            Debug.Log(particleScript.particleType.type);
            Debug.Log(particleScript.particleType.color[0]);
            Debug.Log(particleScript.particleType.color[1]);
            Debug.Log(particleScript.particleType.color[2]);*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}