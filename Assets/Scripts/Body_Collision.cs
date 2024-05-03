using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_Collision : MonoBehaviour
{
    float tagTime = 0;
    float startTime = 0;
    float bufferTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        startTime += Time.deltaTime;
        tagTime += Time.deltaTime;
        bufferTime += Time.deltaTime;
    }

    private void OnCollisionEnter(UnityEngine.Collision other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            if (tagTime >= 5)
            {
                transform.parent.gameObject.GetComponent<CollisionDetection>().collided();
                GetComponent<AudioSource>().Play();
                tagTime = 0;
                bufferTime = 0;
            }
            else
            {
                if ((bufferTime > 2) && (startTime > 10))
                {
                    bufferTime = 0;
                    transform.GetChild(2).GetComponent<AudioSource>().Play();
                }

            }
        }
            
    }
}
