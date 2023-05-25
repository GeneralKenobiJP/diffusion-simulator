using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    private const int NUM_PARTICLES = 500;
    public GameObject Particle;
    public GameObject CalculationProbeObject;
    private StandardBehaviour particleScript;
    public float temperature;
    private string particleType="chlorine"; //private as for now

    private const int Y_PROBE_PRECISION=4;
    private const int ANG_PROBE_PRECISION=4;
    private const int R_PROBE_PRECISION=4;

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

        CylinderScatter(CalculationProbeObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CylinderScatter(GameObject obj)
    {
        var cylinder = GameObject.Find("Cylinder");
        var radius = cylinder.transform.localScale.x/2f;
        var height = cylinder.transform.localScale.y*2f;
        var center = new Vector3(cylinder.transform.position.x,cylinder.transform.position.y,cylinder.transform.position.z);

        var posY=center.y;

        for(posY=center.y;posY<=center.y+height/2;posY+=height/(2f*Y_PROBE_PRECISION))
        {
            CircleScatter(obj);
        }
        for(posY=center.y;posY>=center.y-height/2;posY-=height/(2f*Y_PROBE_PRECISION))
        {
            if(posY==center.y)
                continue;
            CircleScatter(obj);
        }

        void CircleScatter(GameObject obj)
        {
            for(var r=radius/R_PROBE_PRECISION;r<=radius;r+=radius/R_PROBE_PRECISION)
            {
                for(var ang=0f;ang<=Mathf.PI*2f;ang+=Mathf.PI*2f/ANG_PROBE_PRECISION)
                {
                    var obInst = Instantiate(obj);
                    obInst.transform.position = new Vector3(center.x+r*Mathf.Cos(ang),posY,center.z+r*Mathf.Sin(ang));
                }
            }
            var obInstant = Instantiate(obj);
            obInstant.transform.position = new Vector3(center.x,posY,center.z);
        }
    }
}