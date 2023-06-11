using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardBehaviour : MonoBehaviour
{
    public ParticleType particleType;
    public float temperature;
    private float velocity;
    private Vector3 direction = new Vector3();
    public float mass; //molarMass adjusted for cohesion (I guess; not implemented yet)
    public float molarMass=0.032f;
    private double sigma; //standard deviation
    private double mi; //mean
    private Rigidbody rigid;
    private Collider thisCol;
    GameObject Cylinder;
    private float centerDistance; //(considering in XZ plane, then in OY axis) if it exceeds the radius of the cylinder, an emergency function gets it back inside
    public float energyStored=0f;

    private double REFERENCE_MI; //300K
    private const float RESTITUTION_COEF = 0.85f; //(0,1)

    private void SetVelocityStd()
    {
        //Box-Muller Transform
        var u1=UnityEngine.Random.Range(0.001f,1);
        var u2=UnityEngine.Random.Range(0.001f,1);
        var z=Mathf.Sqrt(-2*Mathf.Log(u1))*Mathf.Cos(2*Mathf.PI*u2);
        velocity = (float)(z*sigma+mi);
        //Debug.Log(velocity);
        
        //cannot do Maxwell-Boltzmann Distribution of Speeds implementation because no, so approximating with Gaussian
        //Debug.Log(velocity);
    }
    private void GenerateDirection()
    {
        direction.Set(UnityEngine.Random.Range(-10f,10f),UnityEngine.Random.Range(-10f,10f),UnityEngine.Random.Range(-10f,10f));
        direction.Normalize();
        var relativeVelocity = 0.5f*velocity/(float)CalculateMi(300); //300K is the reference temperature
        direction.Set(direction.x*relativeVelocity,this.direction.y*relativeVelocity,this.direction.z*relativeVelocity);
        //direction*=0.2f;
        /*Debug.Log(direction);
        Debug.Log(direction.magnitude);
        Debug.Log(direction.x);
        Debug.Log(direction.y);
        Debug.Log(direction.z);*/
    }

    /*private void SetDirection() //velocity should have changed by now
    {
        direction.Normalize();
    }*/

    private double CalculateMi(float temp) //we might want actual temperature or reference temperature 300K
    {
        return Math.Sqrt(2*PhysicalConst.GAS_CONSTANT*temp/molarMass);
    }

    // Start is called before the first frame update
    void Start()
    {
        REFERENCE_MI=CalculateMi(300);
        this.transform.localScale=new Vector3(0.05f,0.05f,0.05f);
        //this.transform.position=new Vector3(0f, 3.2f, -7.53f);
        this.transform.position = Setup.GeneratePosition();
        //mass=molarMass*PhysicalConst.uToKg;
        mi=CalculateMi(temperature);
        //mi=0f;
        sigma = Math.Sqrt(PhysicalConst.BOLTZMANN*temperature/molarMass);
        sigma*=1e+11; //making sigma more sensible for the Gaussian approach
        //Debug.Log(sigma);
        //Debug.Log(mi);
        SetVelocityStd();
        GenerateDirection();
        rigid = this.GetComponent<Rigidbody>();
        thisCol = this.GetComponent<Collider>();
        rigid.WakeUp();
        rigid.isKinematic = true;

        Cylinder = GameObject.Find("Cylinder");

        //StartCoroutine(DebugRunTime());

        /*particleType = new ParticleType("Oxygen");
        particleType.boilingPoint="5";
        particleType.meltingPoint="3";
        particleType.bondType="ionic";
        particleType.color="[25,36,78]";
        particleType.colorGas="[29,99,156]";
        particleType.dipoleMoment="4";
        particleType.molarHeatCapacity="30";
        particleType.molarMass="60";
        //particleType.ToStringDebug();
        ParticleType.SaveIntoJSON(particleType);
        particleType = new ParticleType();
        particleType = ParticleType.CreateFromJSON("Oxygen");*/
        //particleType.ToStringDebug();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(direction.magnitude);
        //this.transform.Translate(direction*Time.deltaTime);
        //Debug.DrawRay(this.transform.position, direction * 10f, new Color(0,0,0), 1f);
    }

    IEnumerator DebugRunTime()
    {
        //DebugDirection();
        while(true)
        {
            Debug.Log(direction.magnitude);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void FixedUpdate()
    {
        centerDistance = new Vector3(this.transform.position.x-Cylinder.transform.position.x,0,this.transform.position.z-Cylinder.transform.position.z).magnitude;
        if(centerDistance>Cylinder.transform.localScale.x/2f)
            EmergencyTrigger();
        else
        {
            centerDistance = Mathf.Abs(this.transform.position.y-Cylinder.transform.position.y);
            if(centerDistance>Cylinder.transform.localScale.y)
                EmergencyTrigger();
        }
        this.transform.Translate(direction*Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        var contactPoint = new Vector3();
        var normal = new Vector3();
        if(col.tag=="Vessel")
        {
            contactPoint = col.ClosestPoint(this.transform.position);
            normal = GameObject.Find("Cylinder").GetComponent<VesselCollider>().CalculateNormal(contactPoint);
            ExtendedMath.ReflectVector(ref direction,normal);
            rigid.Sleep();
        } //unused as for now, I think
        else if(col.tag=="GuardVessel")
        {
            contactPoint = col.ClosestPoint(this.transform.position);
            normal = GameObject.Find("Cylinder").GetComponent<VesselCollider>().CalculateNormal(contactPoint);
            ExtendedMath.ReflectVector(ref direction,normal);
            var dir=direction;
            dir.Normalize();
            this.transform.Translate(5f*dir*this.transform.localScale.x*Time.deltaTime);
            rigid.Sleep();
        }
        else if(col.tag=="Particle")
        {
            CollideInelastic(col);
            //Debug.Log("pow pow pow");
        }

    }

    private void EmergencyTrigger()
    {
        direction *= -1f;
        if(!((this.transform.position-Cylinder.transform.position).magnitude>1.13f))
            this.transform.Translate(2f*direction*Time.deltaTime); //making sure it doesn't bounce back
        else
            this.transform.position = Cylinder.transform.position; //rescue where all hope is lost, unbehaving little rascals
    }

    private void OnTriggerExit()
    {
        rigid.WakeUp();
    }

    private void CollideInelastic(Collider col) //calculating only for this particular particle (the other particle calls its own CollideInelastic() function)
    {
        var colScript = col.GetComponent<StandardBehaviour>();
        var colMass = colScript.mass;
        var colEnergyStored = colScript.energyStored*RESTITUTION_COEF;
        var colDirection = colScript.GetDirectionVector();

        colScript.energyStored-=colEnergyStored;
        if(colScript.energyStored<0f)
            colScript.energyStored=0f;

        var newDir = new Vector3();
        newDir = (colDirection-direction)*colMass+mass*direction+colMass*colDirection;
        newDir /= (mass+colMass);
        var frameEnergy = newDir.magnitude;
        newDir *= RESTITUTION_COEF*(1f+colEnergyStored);

        energyStored+=(frameEnergy-newDir.magnitude);

        direction = newDir;

        //Debug.Log(energyStored);
    }

    public void ApplyForce(Vector3 force)
    {
        direction += 0.001f*force*Time.deltaTime;
        if(direction.magnitude>1f)
        {
            //direction.Normalize();
            direction*=0.6f;
        }/*
        else if(direction.magnitude>0.8f)
            direction*=0.7f;
        else if(direction.magnitude>0.65f)
            direction*=0.85f;*/
        //Debug.Log("Applied force");
        //Debug.DrawRay(this.transform.position,force*10f,Color.blue,4f);
        //velocity
    }
    public void ApplyForceMass(Vector3 force)
    {
        direction += 0.000005f*force*Time.deltaTime/mass;
        if(direction.magnitude>1f)
        {
            //direction.Normalize();
            direction*=0.6f;
        }
    }

    public void AssignColor()
    {
        //Debug.Log(particleType.color[0]);
        //Debug.Log(particleType.color[1]);
        //Debug.Log(particleType.color[2]);
        var thisMaterial = this.GetComponent<Renderer>().material;
        thisMaterial.color = new Color32(particleType.color[0],particleType.color[1],particleType.color[2],255);
    }

    public Vector3 GetDirectionVector()
    {
        return direction;
    }

    public void DebugDirection()
    {
        //Debug.Log(direction.x);
        //Debug.Log(direction.y);
        //Debug.Log(direction.z);
        Debug.Log(direction.magnitude);
    }
}
