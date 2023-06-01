using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationProbe : MonoBehaviour
{
    public List<CalculationProbe> columnListHigher;
    public List<CalculationProbe> columnListLower;
    public float probeRadius;
    public List<ParticleType> substances;
    public List<StandardBehaviour> colliderScriptList;
    private float pressureForce; //positive for up, negative for down
    private float[] massCenter; //array of centers of the masses of physical systems consisting of particular particle type (substance)
    // Start is called before the first frame update
    void Start()
    {
        pressureForce=0f;
        Probe();
        StartCoroutine(Compute());
    }

    // Update is called once per frame
    void Update()
    {
        //AddPressure();
        //Probe();
    }

    IEnumerator Compute()
    {
        var timeDelay = new WaitForSeconds(0.2f);
        
        while(true)
        {
            Probe();
            AddPressure();
            //Debug.Log("Compute");
            yield return timeDelay;
        }
    }

    void Probe()
    {
        Collider[] colliderArray;
        colliderArray = Physics.OverlapSphere(this.transform.position,probeRadius);
        colliderScriptList = new List<StandardBehaviour>();
        if(colliderArray.Length>0)
        {
            foreach(var item in colliderArray)
            {
                if(item.tag=="Particle")
                {
                    colliderScriptList.Add(item.GetComponent<StandardBehaviour>());
                    var i = GetSubstanceNum(item);
                    //Debug.Log(i);
                }
            }
            ApplyPressure();
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

    private void AddCohesion()
    {

    }

    private void AddPressure()
    {
        var s=0f;
        foreach(var item in columnListHigher)
        {
            s+=item.CalculateWeight(item.colliderScriptList);
            //Debug.Log(s);
        }
        foreach(var item in columnListLower)
        {
            s-=item.CalculateWeight(item.colliderScriptList);
            //Debug.Log(s);
        }
        pressureForce=s;
    }

    private float CalculateWeight(List<StandardBehaviour> scriptList)
    {
        var s=0f;
        foreach (var item in scriptList)
        {
            s+=item.mass;
        }
        return s;
    }

    private void ApplyPressure()
    {
        var pressureVector = new Vector3(0,1f,0);
        pressureVector *= pressureForce;
        //Debug.Log(pressureForce);
        //pressureVector *= 10f;
        foreach(var item in colliderScriptList)
        {
            item.ApplyForce(pressureVector);
        }
    }
}