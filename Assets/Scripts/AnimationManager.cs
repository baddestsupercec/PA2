using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Rigidbody rigid;

    [SerializeField] GameObject model;

    Animator anim;

    [SerializeField] GameObject body;

    private float xCenter;
    private float zCenter;
    private float prevxCenter;
    private float prevzCenter;

    // Start is called before the first frame update
    void Start()
    {
        //rigid = GetComponent<Rigidbody>();
        xCenter = rigid.worldCenterOfMass.x;
        zCenter = rigid.worldCenterOfMass.z;
        prevxCenter = rigid.worldCenterOfMass.x;
        prevzCenter = rigid.worldCenterOfMass.z;
        anim = model.GetComponent<Animator>();

        InvokeRepeating("animate", 0, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = new Vector3(body.transform.position.x, body.transform.position.y-0.45f, body.transform.position.z);
        model.transform.position = newPos;
        model.transform.rotation = model.transform.parent.rotation;
       // Vector3 newRot = new Vector3(-transform.eulerAngles.x, transform.eulerAngles.y, -transform.eulerAngles.z);
        //model.transform.eulerAngles = newRot;
        //model.transform.rotation = Quaternion.Euler(new Vector3(0, body.transform.rotation.y, 0));
    }

    void animate ()
    {
    xCenter = rigid.worldCenterOfMass.x;
    zCenter = rigid.worldCenterOfMass.z;
    Vector3 facing = rigid.rotation * Vector3.forward;
    Vector3 velocity = rigid.velocity;
    Vector3 massDifference = new Vector3(xCenter - prevxCenter, 0, zCenter - prevzCenter);
    float angleDifference = Vector3.Angle(facing, massDifference);
    float relativeAngleDifference = Vector3.SignedAngle(facing, massDifference, Vector3.up);

    if ((Mathf.Abs(xCenter - prevxCenter) > 0.1) || (Mathf.Abs(zCenter - prevzCenter) > 0.1))
    {
        if ((relativeAngleDifference >= 135) || (relativeAngleDifference <= -135))
        {
            anim.Play("Walking Backwards (1)");
        }
        else if ((relativeAngleDifference > 45) && (relativeAngleDifference < 135))
        {
            anim.Play("Right Strafe Walking");
        }
        else if ((relativeAngleDifference < -45) && (relativeAngleDifference > -135))
        {
            anim.Play("Left Strafe Walking");
        }
        else
        {
            anim.Play("Standard Walk");
        }
    }
        //else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Standard Walk"))
        else
        {
            anim.Play("Breathing Idle");
        }

        prevxCenter = xCenter;
        prevzCenter = zCenter;
    }
}
