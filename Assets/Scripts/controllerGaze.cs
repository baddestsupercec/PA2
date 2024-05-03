using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class controllerGaze : MonoBehaviour
{
    // Start is called before the first frame update
    public bool canSelect;
    public XRRayInteractor ray;
    void Start()
    {
        canSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit obj;
        if (ray.TryGetCurrent3DRaycastHit(out obj)) {
            if(obj.collider.gameObject.transform.tag == "Ball"){
                Debug.Log("SEE BALL");
                canSelect = true;
            }
            else{
                 canSelect = false;
            }   
        }
        else{
            canSelect = false;
        }
    }
}
