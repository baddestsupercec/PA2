using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collider){
        if(collider.gameObject.transform.tag == "Ball"){
            GameObject ball = collider.gameObject;
            ball.GetComponent<Rigidbody>().AddForce(-collider.contacts[0].normal/4,ForceMode.Impulse);
        }
    }
}
