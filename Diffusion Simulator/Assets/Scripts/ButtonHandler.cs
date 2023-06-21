using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    private string topParticle="oxygen";
    private string bottomParticle="oxygen";
    public float temperature=300f;

    public void UpdateParticleSelection(string newParticle, bool isBottom)
    {
        if(isBottom)
            bottomParticle=newParticle;
        else
            topParticle=newParticle;
    }

    public void ReInitializeDiffusion()
    {
        GameObject.FindGameObjectWithTag("Setup").GetComponent<Setup>().ReInitialize(topParticle,bottomParticle,temperature);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
