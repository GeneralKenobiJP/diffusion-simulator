using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationProbe : MonoBehaviour
{
    private List<GameObject> probeList;
    public float probeRadius;
    public List<ParticleType> substances;
    private float[] massCenter; //array of centers of the masses of physical systems consisting of particular particle type (substance)
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Probe();
    }

    void Probe()
    {
        Collider[] colliderArray;
        colliderArray = Physics.OverlapSphere(this.transform.position,probeRadius);
        if(colliderArray.Length>0)
        {
            foreach(var item in colliderArray)
            {
                if(item.tag=="Particle")
                {
                    var i = GetSubstanceNum(item);
                    //Debug.Log(i);
                }
            }
        }
    }

    void InitializeMassCenter()
    {

    }

    private int GetSubstanceNum(Collider collider)
    {
        var i=0;
        foreach(var item in substances)
        {
            //Debug.Log(item.type);
            //Debug.Log(collider.GetComponent<StandardBehaviour>().particleType.type);
            if(item.type==collider.GetComponent<StandardBehaviour>().particleType.type)
                return i;
            i++;
        }
        return 0; //emergency return
    }

    void AddCohesion()
    {

    }
}
