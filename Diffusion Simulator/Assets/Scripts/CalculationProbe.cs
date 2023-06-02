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
    public List<List<StandardBehaviour>> colliderScriptListSortedForSubstances;
    private float pressureForce; //positive for up, negative for down
    private Vector3[] massCenter; //array of centers of the masses of physical systems consisting of particular particle type (substance)
    // Start is called before the first frame update
    void Start()
    {
        pressureForce=0f;
        SetupColliderScriptListSorted();
        Probe();
        StartCoroutine(Compute());
    }

    private void SetupColliderScriptListSorted()
    {
        colliderScriptListSortedForSubstances = new List<List<StandardBehaviour>>();
        foreach(var item in substances)
            colliderScriptListSortedForSubstances.Add(new List<StandardBehaviour>());
        massCenter = new Vector3[substances.Count];
        for(var i=0;i<substances.Count;i++)
        {
            massCenter[i] = new Vector3(0f,0f,0f);
        }
    }
    private void CleanColliderScriptListSorted()
    {
        for(var i=0;i<substances.Count;i++)
            colliderScriptListSortedForSubstances[i] = new List<StandardBehaviour>();

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
            ApplyPressure();
            ApplyCohesion();
            //Debug.Log("Compute");
            yield return timeDelay;
        }
    }

    void Probe()
    {
        Collider[] colliderArray;
        colliderArray = Physics.OverlapSphere(this.transform.position,probeRadius);
        colliderScriptList = new List<StandardBehaviour>();
        var localMass = new float[substances.Count];
        CleanColliderScriptListSorted();
        if(colliderArray.Length>0)
        {
            foreach(var item in colliderArray)
            {
                if(item.tag=="Particle")
                {
                    var itemStandardBehaviour = item.GetComponent<StandardBehaviour>();
                    colliderScriptList.Add(itemStandardBehaviour);
                    var i = GetSubstanceNum(item);
                    //Debug.Log(i);
                    colliderScriptListSortedForSubstances[i].Add(itemStandardBehaviour);
                    AddToMass(item,i,ref localMass[i]);
                }
            }
            AddPressure();
            FinalizeMass(localMass);
            AddCohesion();
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
    ///ADD FORCES (used to calculate forces before applying them)
    private void AddToMass(Collider thisParticle, int substanceNum, ref float localMass)
    {
        var thisMass = thisParticle.GetComponent<StandardBehaviour>().mass;
        massCenter[substanceNum] = thisParticle.transform.position*thisMass;
        localMass+=thisMass;
    }
    private void FinalizeMass(float[] localMass)
    {
        for(var i=0;i<substances.Count;i++)
        {
            massCenter[i]/=localMass[i];
        }
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
    ///APPLY FORCES (Used to influence particles)
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
    private void ApplyGravity()
    {
        var gravityVector = new Vector3(0,-0.1f,0);
        foreach(var item in colliderScriptList)
        {
            gravityVector *= item.mass;
            item.ApplyForce(gravityVector);
        }
    }

    private void ApplyCohesion()
    {
        
    }
}