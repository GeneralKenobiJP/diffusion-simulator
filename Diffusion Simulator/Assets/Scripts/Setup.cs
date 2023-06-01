using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 400;
    private int[] numParticles={200,200};
    public List<ParticleType> substanceArray;
    public List<CalculationColumn> calculationColumns = new List<CalculationColumn>();
    public GameObject Particle;
    public GameObject CalculationProbeObject;
    private StandardBehaviour particleScript;
    public float temperature;
    public List<string> particleType;

    private const int Y_PROBE_PRECISION=6;
    private const int ANG_PROBE_PRECISION=4;
    private const int R_PROBE_PRECISION=2;
    private /*const*/ float PROBE_RADIUS_MINIMUM;

    // Start is called before the first frame update
    void Start()
    {
        var a = "water";
        var b = "chlorine";
        particleType.Add(a);
        particleType.Add(b);
        LoadSubstances();
        for(var i=0;i<NUM_PARTICLES;i++)
        {
            var thisParticle = Instantiate(Particle);
            thisParticle.tag="Particle";
            //var particleGraphics = thisParticle.GetComponent<MeshRenderer>();
            //Destroy(particleGraphics); //makes particles invisible; nice for performance, I hope (maybe not)
            particleScript = thisParticle.GetComponent<StandardBehaviour>();
            particleScript.temperature=temperature;
            AssignSubstances(particleScript, i);
            //particleScript.particleType=jsonSerializer.SearchForParticle(particleType);
            /*Debug.Log(particleScript.particleType.boilingPoint);
            Debug.Log(particleScript.particleType.type);
            Debug.Log(particleScript.particleType.color[0]);
            Debug.Log(particleScript.particleType.color[1]);
            Debug.Log(particleScript.particleType.color[2]);*/

            if(i==0)
                StartCoroutine(DebugOneParticle(particleScript));
        }

        CylinderScatter(CalculationProbeObject);

        DistributeColumnLists();

    }

    IEnumerator DebugOneParticle(StandardBehaviour obj)
    {
        while(true)
        {
            obj.DebugDirection();
            yield return new WaitForSeconds(0.2f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Particle.transform.position);
        //Debug.DrawRay(Particle.transform.position,new Vector3(0,1,0),Color.blue,10f);
    }

    private void CylinderScatter(GameObject obj)
    {
        var cylinder = GameObject.Find("Cylinder");
        var radius = cylinder.transform.localScale.x/2f;
        var height = cylinder.transform.localScale.y*2f;
        var center = new Vector3(cylinder.transform.position.x,cylinder.transform.position.y,cylinder.transform.position.z);

        var posY=center.y;

        var probeRadius = 0f;
        PROBE_RADIUS_MINIMUM = Mathf.Max(2f*height/Y_PROBE_PRECISION,radius/R_PROBE_PRECISION);
        //var probeRadius = Mathf.Max(2f*height/Y_PROBE_PRECISION,radius/R_PROBE_PRECISION,2f*radius/R_PROBE_PRECISION*Mathf.Sin(2f*Mathf.PI/ANG_PROBE_PRECISION)/*distance between two points sharing r and having different angle*/);
        Debug.Log(2f*height/Y_PROBE_PRECISION);
        Debug.Log(radius/R_PROBE_PRECISION);
        Debug.Log(2f*radius/R_PROBE_PRECISION*Mathf.Sin(2f*Mathf.PI/ANG_PROBE_PRECISION));

        for(posY=center.y;posY<=center.y+height/2;posY+=height/Y_PROBE_PRECISION)
        {
            CircleScatter(obj);
        }
        for(posY=center.y;posY>=center.y-height/2;posY-=height/Y_PROBE_PRECISION)
        {
            if(posY==center.y)
                continue;
            CircleScatter(obj);
        }

        void CircleScatter(GameObject obj)
        {
            var i = 0; //iterator for calculation columns
            for(var r=radius/R_PROBE_PRECISION;r<=radius;r+=radius/R_PROBE_PRECISION)
            {
                for(var ang=0f;ang<=Mathf.PI*2f;ang+=Mathf.PI*2f/ANG_PROBE_PRECISION)
                {
                    var obInst = Instantiate(obj);
                    obInst.transform.position = new Vector3(center.x+r*Mathf.Cos(ang),posY,center.z+r*Mathf.Sin(ang));
                    probeRadius = Mathf.Max(PROBE_RADIUS_MINIMUM,2f*radius/R_PROBE_PRECISION*Mathf.Sin(2f*Mathf.PI/ANG_PROBE_PRECISION)/*distance between two points sharing r and having different angle*/);
                    Debug.Log(probeRadius);
                    obInst.GetComponent<CalculationProbe>().probeRadius=probeRadius;
                    obInst.GetComponent<CalculationProbe>().substances=substanceArray;
                    //Debug.Log(obInst.GetComponent<CalculationProbe>().substances[0].type);
                    //Debug.Log(obInst.GetComponent<CalculationProbe>().substances[1].type);

                    //Debug.Log(calculationColumns.Count);
                    if(calculationColumns.Count<=i)
                    {
                        var newColumn = new CalculationColumn();
                        calculationColumns.Add(newColumn);
                    }
                    //Debug.Log("later"+calculationColumns.Count);
                    //Debug.Log(calculationColumns[i].probeList);
                    //Debug.Log("Now"+obInst.GetComponent<CalculationProbe>());
                    calculationColumns[i].probeList.Add(obInst.GetComponent<CalculationProbe>());
                    i++;
                }
            }
            //var obInstant = Instantiate(obj);
            //obInstant.transform.position = new Vector3(center.x,posY,center.z);
        }
    }

    public void LoadSubstances()
    {
        var i=0;
        Debug.Log(particleType);
        foreach(string item in particleType)
        {
            Debug.Log("srutututu: " + i);
            i++;
            Debug.Log(item);
            Debug.Log(jsonSerializer.SearchForParticle(item).type);
            //var x = jsonSerializer.SearchForParticle(item);
            substanceArray.Add(jsonSerializer.SearchForParticle(item));
        }
    }

    public void AssignSubstances(StandardBehaviour script, int num)
    {
        /*foreach(var item in particleType)
        {
            Debug.Log(item);
            Debug.Log(jsonSerializer.SearchForParticle(item).type);
            //var x = jsonSerializer.SearchForParticle(item);
            substanceArray.Add(jsonSerializer.SearchForParticle(item));
        }*/
        var j=0;
        while(num>0)
        {
            if(num>numParticles[j])
                num-=numParticles[j];
            else
            {
                break;
            }
            j++;
        }
        script.particleType = substanceArray[j];
        script.mass = SetMass();

        float SetMass()
        {
            return script.molarMass; //as for now
        }
    }

    private void DistributeColumnLists()
    {
        foreach(var item in calculationColumns)
            foreach(var subItem in item.probeList)
            {
                subItem.columnListHigher = item.GetProbesColumn(subItem.transform.position.y,"higher");
                subItem.columnListLower = item.GetProbesColumn(subItem.transform.position.y,"lower");
                //Debug.Log(subItem.columnListHigher);
                //Debug.Log(subItem.columnListLower);
            }
    }
}