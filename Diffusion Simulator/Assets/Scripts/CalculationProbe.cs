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
    private float[] cohesionForceComponent;
    private float[] adhesionForceComponent;
    private float[] cohesionForce; //non-negative
    private float[] adhesionForce; //non-negative
    private Vector3[] massCenter; //array of centers of the masses of physical systems consisting of particular particle type (substance)
    // Start is called before the first frame update
    private float temperature;
    void Start()
    {
        pressureForce = 0f;
        temperature = GameObject.FindWithTag("Setup").GetComponent<Setup>().temperature;
        SetupForces();
        //Debug.Log(cohesionForceComponent[0]);
        //Debug.Log(cohesionForceComponent[1]);
        Probe();
        StartCoroutine(Compute());
    }

    private void SetupForces()
    {
        colliderScriptListSortedForSubstances = new List<List<StandardBehaviour>>();
        foreach (var item in substances)
            colliderScriptListSortedForSubstances.Add(new List<StandardBehaviour>());
        massCenter = new Vector3[substances.Count];
        cohesionForceComponent = new float[substances.Count];
        adhesionForceComponent = new float[substances.Count * (substances.Count - 1) / 2];
        cohesionForce = new float[substances.Count];
        adhesionForce = new float[substances.Count * (substances.Count - 1) / 2];
        for (var i = 0; i < substances.Count; i++)
        {
            massCenter[i] = new Vector3(0f, 0f, 0f);
            cohesionForceComponent[i] = SetupCohesionComponent(i);
            Debug.Log(cohesionForceComponent[i]);
        }
        for (var i = 0; i < substances.Count * (substances.Count - 1) / 2; i++)
        {
            adhesionForceComponent[i] = SetupAdhesionComponent(i);
            //Debug.Log(adhesionForceComponent[i]);
        }

        float SetupCohesionComponent(int j)
        {
            var coh = 4f;
            coh += (0.2f * substances[j].normalDensity);
            if (substances[j].dipoleMoment > 0)
                coh *= substances[j].dipoleMoment;

            switch (substances[j].bondType)
            {
                case "metallic":
                    coh *= 5f;
                    break;
                case "ionic":
                    coh *= 3f;
                    break;
                case "polar covalent":
                    coh *= 1.25f;
                    break;
                default: //non-polar covalent
                    coh *= 0.5f;
                    break;
            }
            coh *= 0.1f;
            //Debug.Log(coh);
            return coh;
        }
        float SetupAdhesionComponent(int j)
        {
            var adh = 2.5f;
            var subNum = GetSubstanceNumPair(j);
            //Debug.Log(subNum[0]);
            //Debug.Log(subNum[1]);
            adh -= 0.1f * substances[subNum[0]].normalDensity;
            adh -= 0.1f * substances[subNum[1]].normalDensity;
            if (adh < 0)
                adh = 0.05f;
            if (substances[subNum[0]].dipoleMoment > 0)
                adh *= Mathf.Sqrt(substances[subNum[0]].dipoleMoment);
            if (substances[subNum[1]].dipoleMoment > 0)
                adh *= Mathf.Sqrt(substances[subNum[1]].dipoleMoment);

            switch (substances[subNum[0]].bondType)
            {
                case "metallic":
                    adh *= 0.4f;
                    break;
                case "ionic":
                    adh *= 2.5f;
                    break;
                case "polar covalent":
                    adh *= 2f;
                    break;
                default: //non-polar covalent
                    adh *= 0.5f;
                    break;
            }
            switch (substances[subNum[1]].bondType)
            {
                case "metallic":
                    adh *= 0.4f;
                    break;
                case "ionic":
                    adh *= 2.5f;
                    break;
                case "polar covalent":
                    adh *= 2f;
                    break;
                default: //non-polar covalent
                    adh *= 0.5f;
                    break;
            }
            adh *= 0.1f;

            return adh;
        }
    }
    private void CleanColliderScriptListSorted()
    {
        for (var i = 0; i < substances.Count; i++)
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

        while (true)
        {
            Probe();
            ApplyPressure();
            //ApplyCohesion();
            //Debug.Log(cohesionForce[0]);
            //Debug.Log(pressureForce);
            //Debug.Log(cohesionForce[1]);
            yield return timeDelay;
        }
    }

    void Probe()
    {
        Collider[] colliderArray;
        colliderArray = Physics.OverlapSphere(this.transform.position, probeRadius);
        colliderScriptList = new List<StandardBehaviour>();
        var localMass = new float[substances.Count];
        CleanColliderScriptListSorted();
        if (colliderArray.Length > 0)
        {
            foreach (var item in colliderArray)
            {
                if (item.tag == "Particle")
                {
                    var itemStandardBehaviour = item.GetComponent<StandardBehaviour>();
                    colliderScriptList.Add(itemStandardBehaviour);
                    var i = GetSubstanceNum(item);
                    //Debug.Log(i);
                    colliderScriptListSortedForSubstances[i].Add(itemStandardBehaviour);
                    AddToMass(item, i, ref localMass[i]);
                    //Debug.Log(localMass[0]);
                }
            }
            AddPressure();
            //FinalizeMass(localMass);
            /*Debug.Log(massCenter[0].x);
            Debug.Log(massCenter[0].y);
            Debug.Log(massCenter[0].z);
            Debug.Log(massCenter[1].x);
            Debug.Log(massCenter[1].y);
            Debug.Log(massCenter[1].z);*/
            AddCohesion(localMass);
            AddAdhesion(localMass);
        }
    }

    void InitializeMassCenter()
    {

    }

    private int GetSubstanceNum(Collider collider)
    {
        var i = 0;
        foreach (var item in substances)
        {
            //Debug.Log(item.type);
            //Debug.Log(collider.GetComponent<StandardBehaviour>().particleType.type);
            if (item.type == collider.GetComponent<StandardBehaviour>().particleType.type)
                return i;
            i++;
        }
        return 0; //emergency return
    }
    private int GetSubstanceNum(StandardBehaviour thisScript)
    {
        var i = 0;
        foreach (var item in substances)
        {
            if (item.type == thisScript.particleType.type)
                return i;
            i++;
        }
        return 0; //emergency return
    }
    int[] GetSubstanceNumPair(int j)
    {
        var x = 1;
        var k = 0;
        var N = substances.Count;
        //var M = N*(N-1)/2;
        while (k + (N - x) < j)
        {
            k += N - x;
            x++;
        }
        return new int[2] { x - 1, j - k + x };
    }
    ///
    ///ADD FORCES (used to calculate forces before applying them)
    ///
    private void AddToMass(Collider thisParticle, int substanceNum, ref float localMass)
    {
        var thisMass = thisParticle.GetComponent<StandardBehaviour>().mass; //actually the masses do not matter (m/m)
        massCenter[substanceNum] = thisParticle.transform.position;
        localMass += thisMass;
    }
    private void FinalizeMass(float[] localMass) //unneeded
    {
        for (var i = 0; i < substances.Count; i++)
        {
            massCenter[i] /= localMass[i];
        }
    }
    private void AddCohesion(float[] localMass)
    {
        //Debug.Log(cohesionForceComponent[0]);
        var thisForce = cohesionForceComponent;
        for (var i = 0; i < substances.Count; i++)
        {
            if (temperature >= substances[i].boilingPoint)
                thisForce[i] *= 0.25f;
            else if (temperature < substances[i].meltingPoint)
                thisForce[i] *= 10f;
            thisForce[i] *= Mathf.Sqrt(localMass[i]);
            thisForce[i] *= 0.5f;
            cohesionForce[i] = thisForce[i];
            //Debug.Log(cohesionForce[i]);
        }
        //Debug.Log(cohesionForce[0]);
    }
    private void AddAdhesion(float[] localMass)
    {
        var thisForce = adhesionForceComponent;
        for (var i = 0; i < substances.Count * (substances.Count - 1) / 2; i++)
        {
            var subNum = GetSubstanceNumPair(i);
            if(temperature >= substances[subNum[0]].boilingPoint)
                thisForce[i] *= 0.2f;
            if(temperature >= substances[subNum[1]].boilingPoint)
                thisForce[i] *= 0.2f;
            //Debug.Log(thisForce[i]);
            var surfaceForce = Mathf.Abs(cohesionForceComponent[subNum[0]]-cohesionForceComponent[subNum[1]]);
            thisForce[i]+=surfaceForce;
            //ADD THE MASS THINGY
            adhesionForce[i]=thisForce[i];
        }
    }
    private void AddPressure()
    {
        var s = 0f;
        foreach (var item in columnListHigher)
        {
            s += item.CalculateWeight(item.colliderScriptList);
            //Debug.Log(s);
        }
        foreach (var item in columnListLower)
        {
            s -= item.CalculateWeight(item.colliderScriptList);
            //Debug.Log(s);
        }
        pressureForce = s;
    }

    private float CalculateWeight(List<StandardBehaviour> scriptList)
    {
        var s = 0f;
        foreach (var item in scriptList)
        {
            s += item.mass;
        }
        return s;
    }
    ///
    ///APPLY FORCES (Used to influence particles)
    ///
    private void ApplyPressure()
    {
        var pressureVector = new Vector3(0, 1.1f, 0);
        pressureVector *= pressureForce;
        //Debug.Log(pressureForce);
        //pressureVector *= 10f;
        foreach (var item in colliderScriptList)
        {
            item.ApplyForce(pressureVector);
        }
    }
    private void ApplyGravity()
    {
        var gravityVectorTemp = new Vector3(0, -0.1f, 0);
        foreach (var item in colliderScriptList)
        {
            var gravityVector = gravityVectorTemp * item.mass;
            item.ApplyForce(gravityVector);
        }
    }

    private void ApplyCohesion()
    {
        var cohesionVector = new Vector3();
        foreach (var item in colliderScriptList)
        {
            var i = GetSubstanceNum(item);
            cohesionVector = massCenter[i] - item.transform.position;
            cohesionVector.Normalize();
            cohesionVector *= cohesionForce[i];
            item.ApplyForce(cohesionVector);
            //Debug.Log(cohesionVector.magnitude);
        }
    }
}