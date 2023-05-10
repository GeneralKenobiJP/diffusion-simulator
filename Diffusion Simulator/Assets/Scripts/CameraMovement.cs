using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private GameObject RotationCenter; //center of circular rotation, empty gameobject
    private Vector3 StartingPoint;
    private Vector3 RotationRadius;
    private float cameraAngle;
    // Start is called before the first frame update
    void Start()
    {
        cameraAngle = this.transform.rotation.eulerAngles.x;
        RotationCenter = new GameObject();
        var cylinderPos = GameObject.Find("Cylinder").transform.position;
        RotationCenter.transform.position = new Vector3(cylinderPos.x,this.transform.position.y,cylinderPos.z);
        StartingPoint = this.transform.position;
        RotationRadius=StartingPoint-RotationCenter.transform.position;
        Debug.DrawRay(RotationCenter.transform.position,RotationRadius,new Color(255,0,125),20f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("d"))
        {
            RotationRadius = ExtendedMath.RotateVector2(RotationRadius,0.01f,RotationRadius.y); //radians
            this.transform.position = RotationCenter.transform.position+RotationRadius;
            this.transform.LookAt(RotationCenter.transform);
            this.transform.rotation = Quaternion.Euler(cameraAngle,this.transform.rotation.eulerAngles.y,this.transform.rotation.eulerAngles.z); //I fucking hate quaternions
        }
        if(Input.GetKey("a"))
        {
            RotationRadius = ExtendedMath.RotateVector2(RotationRadius,-0.01f,RotationRadius.y);
            this.transform.position = RotationCenter.transform.position+RotationRadius;
            this.transform.LookAt(RotationCenter.transform);
            this.transform.rotation = Quaternion.Euler(cameraAngle,this.transform.rotation.eulerAngles.y,this.transform.rotation.eulerAngles.z);
        }
    }
}
