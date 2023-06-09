using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesselCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private float radius;
    private float height;
    private Vector3 baseCenter;
    private  Vector3 center;
    private const int PRECISION=32; //how many plane-colliders do we create?
    private GameObject planeCollider;
    private MeshCollider planeMeshCollider;
    private Rigidbody planeRigid;

    public Vector3 CalculateNormal(Vector3 Point)
    {
        var normal = new Vector3();
        if(Point.y<center.y+height/2-0.05f && Point.y>center.y-height/2+0.05f)
        {
            normal = new Vector3(center.x-Point.x,0,center.z-Point.z);
        }
        else
            {normal = new Vector3(0,center.y-Point.y,0);}
        normal.Normalize();
        return normal;
    }

    private void ColliderPopulate(float scale, string colTag)
    {
        var ang = Mathf.PI/PRECISION; //half the central angle
        var degAng = ExtendedMath.RadToDeg(2f*ang); //central angle in degrees
        var h = new Vector3(radius*Mathf.Cos(ang)*scale,0,0); //height vector that shoots from the center to the center of the plane
        var planeScaleX = 0.2f*radius*Mathf.Sin(ang)*scale; //2rsin(ang/2), adjusted for 1x1 plane, *scale
        var planeScaleZ = 0.1f*height*scale;
        var planeScale = new Vector3(planeScaleX,0.01f*scale,planeScaleZ);
        var pos = center+h;
        var basicRot=new Vector3(90,90,180);
        var rot=basicRot;
        var baseScale = new Vector3(0.1f,0.01f,0.1f)*scale;
        // ###
        planeCollider = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeCollider.transform.position = pos;
        planeCollider.transform.Rotate(basicRot);
        planeCollider.transform.localScale = planeScale;
        planeMeshCollider = planeCollider.GetComponent<MeshCollider>();
        planeMeshCollider.tag = colTag;
        planeMeshCollider.convex = true;
        Destroy(planeCollider.GetComponent<MeshRenderer>());
        // ###
        for(var i=2;i<=PRECISION;i++)
        {
            h=ExtendedMath.RotateVector2(h,2*ang);
            pos = center+h;
            rot += new Vector3(0,0,degAng);
            //Debug.Log(rot);
            planeCollider = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeCollider.transform.position = pos;
            planeCollider.transform.Rotate(rot);
            planeCollider.transform.localScale = planeScale;
            planeMeshCollider = planeCollider.GetComponent<MeshCollider>();
            planeMeshCollider.convex = true;
            planeMeshCollider.tag = colTag;
            Destroy(planeCollider.GetComponent<MeshRenderer>());
        }

        planeCollider = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeCollider.transform.position = center+new Vector3(0,height/2*scale,0);
        planeCollider.transform.Rotate(new Vector3(0,0,180));
        planeCollider.transform.localScale = baseScale;
        planeMeshCollider = planeCollider.GetComponent<MeshCollider>();
        planeMeshCollider.convex = true;
        planeMeshCollider.tag = colTag;
        Destroy(planeCollider.GetComponent<MeshRenderer>());

        planeCollider = GameObject.CreatePrimitive(PrimitiveType.Plane);
        planeCollider.transform.position = center-new Vector3(0,height/2*scale,0);
        planeCollider.transform.localScale = baseScale;
        planeMeshCollider = planeCollider.GetComponent<MeshCollider>();
        planeMeshCollider.convex = true;
        planeMeshCollider.tag = colTag;
        Destroy(planeCollider.GetComponent<MeshRenderer>());
    }

    private void SetupColliders()
    {
        ColliderPopulate(1f,"Vessel"); //Inner colliders (those normal)
        //ColliderPopulate(1.12f*1/Mathf.Cos(Mathf.PI/PRECISION),"GuardVessel"); //Outer colliders (last resort colliders for forsaken particles)
    }

    /*private void FlipNormals(MeshCollider MC)
    {
        var M = MC.sharedMesh;
        List<Vector3> currentNormals = new List<Vector3>();
        List<Vector3> flippedNormals = new List<Vector3>();
        M.GetNormals(currentNormals);
        foreach(Vector3 norm in currentNormals)
        {
            flippedNormals.Add(norm*-0.01f);
        }
        M.SetNormals(flippedNormals);
        MC.sharedMesh=M;
        
        Debug.Log(currentNormals);
        foreach(var x in currentNormals)
        {
            Debug.Log(x);
        }
        List<Vector3> y= new List<Vector3>();
        M.GetNormals(y);
        Debug.Log(currentNormals);
        foreach(Vector3 x in y)
        {
            Debug.Log(x);
        }
        //foreach(var nor in M.GetNormals(currentNormals));
    }*/
    void Start()
    {
        radius = this.transform.localScale.x/2f; //assuming a perfect circle
        height = this.transform.localScale.y*2f;
        baseCenter = new Vector3(this.transform.position.x,this.transform.position.y-0.5f*height,this.transform.position.z);
        center = new Vector3(this.transform.position.x,this.transform.position.y,this.transform.position.z); //quicker, handier, better
        SetupColliders();

        /*FlipNormals(this.GetComponent<MeshCollider>());
        List<Vector3> y= new List<Vector3>();
        this.GetComponent<MeshCollider>().sharedMesh.GetNormals(y);
        Debug.Log(y);
        foreach(Vector3 x in y)
        {
            Debug.Log(x);
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
