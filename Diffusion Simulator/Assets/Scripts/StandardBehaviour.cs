using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StandardBehaviour : MonoBehaviour
{
    //private static x = new PhysicalConst();
    public float temperature;
    private float velocity;
    private Vector3 direction = new Vector3();
    private double mass;
    private float molarMass=0.032f;
    private double sigma; //standard deviation
    private double mi; //mean
    private Rigidbody rigid;

    private void SetVelocityStd()
    {
        //Box-Muller Transform
        var u1=UnityEngine.Random.Range(0.001f,1);
        var u2=UnityEngine.Random.Range(0.001f,1);
        var z=Mathf.Sqrt(-2*Mathf.Log(u1))*Mathf.Cos(2*Mathf.PI*u2);
        velocity = (float)(z*sigma+mi);
        Debug.Log(velocity);
        //
        //cannot do Maxwell-Boltzmann Distribution of Speeds implementation because no, so approximating with Gaussian
        //Debug.Log(velocity);
    }
    private void GenerateDirection()
    {
        direction.Set(UnityEngine.Random.Range(1f,10f),UnityEngine.Random.Range(1f,10f),UnityEngine.Random.Range(1f,10f)); //change so it can be negative
        direction.Normalize();
        var relativeVelocity = 0.2f*velocity/(float)mi;
        relativeVelocity*=0.2f; //TEMPORARY
        direction.Set(direction.x*relativeVelocity,this.direction.y*relativeVelocity,this.direction.z*relativeVelocity);
        Debug.Log(direction);
        Debug.Log(direction.magnitude);
        Debug.Log(direction.x);
        Debug.Log(direction.y);
        Debug.Log(direction.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position=new Vector3(0f, 3.2f, -7.53f);
        mass=molarMass*PhysicalConst.uToKg;
        mi=Math.Sqrt(2*PhysicalConst.GAS_CONSTANT*temperature/molarMass);
        //mi=0f;
        sigma = Math.Sqrt(PhysicalConst.BOLTZMANN*temperature/molarMass);
        sigma*=1e+11; //making sigma more sensible for the Gaussian approach
        Debug.Log(sigma);
        Debug.Log(mi);
        SetVelocityStd();
        GenerateDirection();
        rigid = this.GetComponent<Rigidbody>();
        rigid.WakeUp();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(direction*Time.deltaTime);
        Debug.DrawRay(this.transform.position, direction * 10f, new Color(0,0,0), 1f);

        /*RaycastHit hit = new RaycastHit();
        /*if(Physics.Raycast(new Ray(this.transform.position,direction),out hit,200f))
        {
            Debug.Log("Ray in blood");
            Debug.Log(hit);
            Debug.DrawRay(hit.point, -1*hit.normal * 100, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            ReflectVector(ref direction,-1*hit.normal);
        }*//*
        var startPos = this.transform.position;
        var endPos = startPos+direction*5f;
        if(Physics.Linecast(startPos,endPos,out hit))
        {
            Debug.Log("Ray in blood");
            Debug.Log(hit);
            Debug.DrawRay(hit.point, -1*hit.normal * 0.1f, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            ReflectVector(ref direction,-1*hit.normal);
        }*/
    }

    void FixedUpdate()
    {

    }

    /*private void OnTriggerEnter(Collider col)
    {
        if(col.tag=="vessel")
        {

        }
    }*/

    //private void Bounce()
    //{
//
  //  }

    private void OnCollisionEnter(Collision col)
    {
        rigid.Sleep();
        rigid.isKinematic = true;
        Debug.Log(col.collider);
        if(col.collider.tag=="Vessel")
        {
            Debug.Log("Normal of the first point: " + col.contacts[0].normal);
            Debug.Log(col.collider.name);
            Debug.Log(col.contacts[0].normal.x);
            Debug.Log(col.contacts[0].normal.y);
            Debug.Log(col.contacts[0].normal.z);
            foreach (var item in col.contacts)
            {
                Debug.DrawRay(item.point, item.normal * 100, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            }
            Debug.Log(direction);
            Debug.Log(direction.x);
            Debug.Log(direction.y);
            Debug.Log(direction.z);
            ExtendedMath.ReflectVector(ref direction,-col.contacts[0].normal);
            this.transform.Translate(0.1f*direction);
            Debug.Log(direction);
            Debug.Log(direction.x);
            Debug.Log(direction.y);
            Debug.Log(direction.z);
            rigid.Sleep();
            rigid.isKinematic = true;
        }
        //var x = direction;
        //ReflectVector(ref x,col.contacts[0].normal);
        //Debug.Log(x.x);
        //Debug.Log(x.y);
        //Debug.Log(x.z);
        //ExtendedMath.ReflectVector(ref direction,-1*col.contacts[0].normal);
        //rigid.Sleep();
        //rigid.isKinematic=true;
        //rigid.Sleep();
    }
    private void OnCollisionStay(Collision col)
    {
        this.transform.Translate(0.1f*direction);
        /*Debug.Log(col.collider);
        if(col.collider.name=="Cylinder")
        {
            Debug.Log("Normal of the first point: " + col.contacts[0].normal);
            Debug.Log(col.contacts[0].normal.x);
            Debug.Log(col.contacts[0].normal.y);
            Debug.Log(col.contacts[0].normal.z);
            foreach (var item in col.contacts)
            {
                Debug.DrawRay(item.point, -1*item.normal * 100, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            }
        }
        //var x = direction;
        //ReflectVector(ref x,col.contacts[0].normal);
        //Debug.Log(x.x);
        //Debug.Log(x.y);
        //Debug.Log(x.z);
        ReflectVector(ref direction,col.contacts[0].normal);*/
        
        //this.transform.Translate(5f*direction*Time.deltaTime);
    }
    private void OnCollisionExit()
    {
        this.transform.Translate(0.2f*direction);
        Debug.Log("Hakuna matata");
        rigid.isKinematic=false;
        rigid.WakeUp();
    }
}
