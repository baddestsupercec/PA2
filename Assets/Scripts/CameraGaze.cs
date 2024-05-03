using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGaze : MonoBehaviour
{
    // Start is called before the first frame update
    public bool canTeleport;
    public GameObject tpSign;
    void Start()
    {
        canTeleport = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit obj;
        if (Physics.Raycast (transform.position, transform.forward, out obj)) {
            if(obj.collider.gameObject.transform.tag == "Teleport"){
                Debug.Log("SEE BALL");
                canTeleport = true;
                tpSign = obj.collider.gameObject;
            }
            else{
                 canTeleport = false;
            }   
        }
        else{
            canTeleport = false;
        }
    }
}
