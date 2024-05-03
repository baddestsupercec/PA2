using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blade_rotation : MonoBehaviour
{
    //Vector3 rotation;
    // Start is called before the first frame update
    public float speed = new float();
    void Start()
    {
        //rotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //rotation += new Vector3(1,0,0) * Time.deltaTime;
        transform.Rotate(speed * Time.deltaTime, 0.0f, 0.0f, Space.Self);
    }
}
