using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Jobs;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed;
    public Transform tform;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 speed = new Vector3(0, rb.velocity.x, 0);
        Quaternion rot = Quaternion.Euler(speed.x,speed.y,speed.z);

        tform.rotation = Quaternion.RotateTowards(tform.rotation, rot, rotateSpeed);
    }
}
