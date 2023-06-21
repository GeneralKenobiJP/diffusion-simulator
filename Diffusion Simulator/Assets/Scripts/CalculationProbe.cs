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
    private float temperature;
    private ColorPoint[] interpolationPoints;
    private GameObject[] interpolationObjects;
    private Material interpolationMaterial;
    public Vector3 cylinderCenter;
    private const bool IS_VACUUM=false;
    private const float OXYGEN_DENSITY=0.0012f;
    private const float MAX_FORCE=8f;
    private float radialDistance;
    private float deltaAngle;
    private float heightDistance;
    private const int INTERPOLATION_PRECISION=3; //unto how many segments we divide the abovementioned distances
    //^3 = number of interpolationPoints/Objects
    void Start()
    {
        interpolationMaterial = Resources.Load("Materials/Particle.mat", typeof(Material)) as Material;
        pressureForce = 0f;
        temperature = GameObject.FindWithTag("Setup").GetComponent<Setup>().temperature;
        SetupForces();
        SetupInterpolationPoints();
        //Debug.Log(cohesionForceComponent[0]);
        //Debug.Log(cohesionForceComponent[1]);
        Probe();
        StartCoroutine(Compute());
    }

    public void ReInitialize()
    {
        temperature = GameObject.FindWithTag("Setup").GetComponent<Setup>().temperature;
        SetupForces();
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
            //Debug.Log(cohesionForceComponent[i]);
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
            var adh = 2f;
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
                    adh *= 2f;
                    break;
                case "polar covalent":
                    adh *= 1.5f;
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
                    adh *= 2f;
                    break;
                case "polar covalent":
                    adh *= 1.5f;
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

    /// UPDATING SCRIPT
    IEnumerator Compute()
    {
        var timeDelay = new WaitForSeconds(0.2f); //0.2f

        while (true)
        {
            Probe();
            ApplyPressure();
            ApplyCohesion();
            ApplyAdhesion();
            ApplyGravity();
            InterpolateColor();
            //Debug.Log(cohesionForce[0]);
            //Debug.Log(cohesionForce[1]);
            //Debug.Log(adhesionForce[0]);
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
        CleanMassCenter();
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
            FinalizeMass();
            /*for(var i=0;i<substances.Count;i++)
            {
            var temptemp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temptemp.transform.position = massCenter[i];
            temptemp.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            }*/
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

    void CleanMassCenter()
    {
        for (var i = 0; i < substances.Count; i++)
        {
            massCenter[i] = new Vector3(0f, 0f, 0f);
        }
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
    bool IsSubstanceNumInPair(int subNum, int pairNum)
    {
        var pairSubs = GetSubstanceNumPair(pairNum);
        if(subNum == pairSubs[0] || subNum == pairSubs[1])
            return true;
        else
            return false;
    }
    bool IsSubstanceNumInPair(int subNum, int pairNum, out int secondInPair)
    {
        secondInPair = -1;
        var pairSubs = GetSubstanceNumPair(pairNum);
        if(subNum == pairSubs[0])
        {
            secondInPair = pairSubs[1];
            return true;
        }
        else if(subNum == pairSubs[1])
        {
            secondInPair = pairSubs[0];
            return true;
        }
        else
            return false;
    }
    ///
    ///ADD FORCES (used to calculate forces before applying them)
    ///
    private void AddToMass(Collider thisParticle, int substanceNum, ref float localMass)
    {
        var thisMass = thisParticle.GetComponent<StandardBehaviour>().mass; //actually the masses do not matter (m/m)
        massCenter[substanceNum] += thisParticle.transform.position;
        localMass += thisMass;
    }
    private void FinalizeMass()
    {
        for (var i = 0; i < substances.Count; i++)
        {
            massCenter[i] /= colliderScriptListSortedForSubstances[i].Count;
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
                thisForce[i] *= 3.5f;
            thisForce[i] *= Mathf.Sqrt(localMass[i]);
            thisForce[i] *= 0.2f;
            cohesionForce[i] = Mathf.Min(thisForce[i],MAX_FORCE);
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
            thisForce[i]*=surfaceForce;
            thisForce[i]*=0.0001f*Mathf.Abs(substances[subNum[0]].molarMass-substances[subNum[1]].molarMass)*(localMass[subNum[0]]+localMass[subNum[1]]);

            adhesionForce[i]=Mathf.Min(thisForce[i],MAX_FORCE);
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
        var gravityVectorTemp = new Vector3(0, -20f, 0);
        //gravityVectorTemp.y //g*m(1-ro/d), but later we divide by m anyway
        foreach (var item in colliderScriptList)
        {
            var gravityVector = gravityVectorTemp;
            if(!IS_VACUUM)
                gravityVector*=(1f-OXYGEN_DENSITY/item.matterDensity);
            //Debug.Log(gravityVector.y);
            item.ApplyForce(gravityVector);
            //if(item.particleType.type=="hydrogen")
              //  Debug.Log(gravityVector.y);
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
    private void ApplyAdhesion()
    {
        var adhesionVector = new Vector3();
        foreach(var item in colliderScriptList)
        {
            for(var i=0;i<substances.Count*(substances.Count-1)/2;i++)
            {
                var other = new int();
                if(!IsSubstanceNumInPair(GetSubstanceNum(item),i,out other))
                    continue;
                else
                {
                    adhesionVector = massCenter[other] - item.transform.position;
                    adhesionVector.Normalize();
                    adhesionVector *= adhesionForce[i];
                    //Debug.Log(adhesionVector.magnitude);
                    item.ApplyForce(adhesionVector);
                }
            }
        }
    }



    /// INTERPOLATION SECTION ///

    public void SetDistances(float straightDist, float angDist, float hghDist)
    {
        radialDistance = straightDist/(INTERPOLATION_PRECISION); //we need to share space with neighbours
        deltaAngle = angDist/(INTERPOLATION_PRECISION);
        heightDistance = hghDist/(INTERPOLATION_PRECISION);
    }
    private void SetupInterpolationPoints()
    {
        var objNum = INTERPOLATION_PRECISION*INTERPOLATION_PRECISION*INTERPOLATION_PRECISION;
        interpolationPoints = new ColorPoint[objNum];
        interpolationObjects = new GameObject[objNum];
        for(var i=0;i<interpolationPoints.Length;i++)
        {
            interpolationPoints[i] = new ColorPoint();
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(0.03f,0.03f,0.03f);
            //interpolationObjects[0] = new GameObject();
            interpolationObjects[i] = cube;
            interpolationObjects[i].GetComponent<Renderer>().material=interpolationMaterial;
        }
        ScatterPoints();
        for(var i=0;i<interpolationObjects.Length;i++)
            interpolationObjects[i].transform.position = interpolationPoints[i].position;

        void ColorInterpolationPoints() //test&debugging function
        {
            for(var i=0;i<interpolationObjects.Length;i++)
            {
                if((i/9)%3==0)
                {
                    interpolationObjects[i].GetComponent<Renderer>().material.color=new Color(0f,0f,0f);
                }
                else if((i/9)%3==1)
                    interpolationObjects[i].GetComponent<Renderer>().material.color=new Color(1f,0f,0f);
                else
                    interpolationObjects[i].GetComponent<Renderer>().material.color=new Color(1f,1f,1f);

            }
        }

        void ScatterPoints()
        {
            var thisCenter = this.transform.position;
            //we also use:
            //float radialDistance;
            //float deltaAngle;
            //float heightDistance;
            //Vector3 cylinderCenter
            var cylinderCenterAdjusted = new Vector3(cylinderCenter.x,thisCenter.y,cylinderCenter.z);
            var radialVector = thisCenter-cylinderCenterAdjusted;

            var thisPosition = cylinderCenterAdjusted+radialVector*((radialVector.magnitude-0.5f*INTERPOLATION_PRECISION*radialDistance)/radialVector.magnitude);
            var thisVector = radialDistance*radialVector/radialVector.magnitude;
            var n=0;
            thisPosition.y-=heightDistance*0.5f*INTERPOLATION_PRECISION;
            thisPosition = ExtendedMath.RotateVector2AtPoint(thisPosition,-1f*deltaAngle*0.5f*INTERPOLATION_PRECISION,new Vector3(cylinderCenter.x,thisPosition.y,cylinderCenter.z));
            thisVector = ExtendedMath.RotateVector2(thisVector,-1f*deltaAngle*0.5f*INTERPOLATION_PRECISION,thisVector.y);
            for(var i=0;i<INTERPOLATION_PRECISION;i++) //angle incrementation
            {
                var startHeight = thisPosition.y;
                for(var j=0;j<INTERPOLATION_PRECISION;j++) //height incrementation
                {
                    var startPosition = thisPosition;
                    for(var k=0;k<INTERPOLATION_PRECISION;k++) //radius incrementation
                    {
                        interpolationPoints[n].position = thisPosition;
                        thisPosition+=thisVector;
                        n++;
                    }
                    thisPosition = startPosition;
                    thisPosition.y+=heightDistance;
                }
                thisPosition.y=startHeight;
                var thisCenterAdjusted = new Vector3(cylinderCenter.x,startHeight,cylinderCenter.z);
                thisPosition = ExtendedMath.RotateVector2AtPoint(thisPosition,deltaAngle,thisCenterAdjusted);
                thisVector = ExtendedMath.RotateVector2(thisVector,deltaAngle,thisVector.y);
            }
        }
    }
    private void SetupInterpolationPointsTest()
    {
        interpolationPoints = new ColorPoint[1];
        for(var i=0;i<interpolationPoints.Length;i++)
            interpolationPoints[i] = new ColorPoint();
        Debug.Log(interpolationPoints[0].position.x);
        Debug.Log(interpolationPoints[0].position.y);
        Debug.Log(interpolationPoints[0].position.z);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = this.transform.position;
        cube.transform.rotation = this.transform.rotation;
        cube.transform.localScale = new Vector3(0.03f,0.03f,0.03f);
        interpolationObjects = new GameObject[1];
        //interpolationObjects[0] = new GameObject();
        interpolationObjects[0] = cube;
        
        interpolationObjects[0].GetComponent<Renderer>().material=interpolationMaterial;
    }
    private void InterpolateColor()
    {
        if(colliderScriptList.Count>0)
        {
            var inputPoints = new ColorPoint[colliderScriptList.Count];
            var i=0;
            foreach(var item in colliderScriptList)
            {
                inputPoints[i] = new ColorPoint();
                //Debug.Log(item.transform.position);
                inputPoints[i].position = item.transform.position;
                inputPoints[i].colorHSV.SetHSVFromRGB(item.particleType.color);
                i++;
            }
            Interpolator.IDW(inputPoints, ref interpolationPoints);

            //interpolationPoints[0].colorHSV.DebugHSV();
            //Debug.Log(interpolationPoints[0].colorHSV.GetRGBFromHSV().r);
            //Debug.Log(interpolationPoints[0].colorHSV.GetRGBFromHSV().g);
            //Debug.Log(interpolationPoints[0].colorHSV.GetRGBFromHSV().b);
            for(var j=0;j<interpolationObjects.Length;j++)
            {
                interpolationObjects[j].transform.localScale = new Vector3(0.03f,0.03f,0.03f);
                interpolationObjects[j].GetComponent<Renderer>().material.color=interpolationPoints[j].colorHSV.GetRGBFromHSV();
            }
        }
        else
        {
            for(var j=0;j<interpolationObjects.Length;j++)
                interpolationObjects[j].transform.localScale = new Vector3(0f,0f,0f);
            //Debug.Log("le disappear");
        }
    }
}