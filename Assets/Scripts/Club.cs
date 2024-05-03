using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Club : MonoBehaviour
{
    // Start is called before the first frame update
    public int belongToPlayer;
    public bool beenGrabbed;
    public int playerNumber;
    public GameObject ball;
    public GameObject player;
    private float cooldown;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cooldown+=Time.deltaTime;
    }


    public void grabbed(){
        if(!beenGrabbed){
            if(player.GetComponent<MovementManager>().spawnBall()){
            Vector3 pos = transform.position;
            //pos = pos + transform.forward*2;
            MovementManager playerScript = player.GetComponent<MovementManager>();
            pos = playerScript.spawns[playerScript.curCourse].transform.position;
            pos.y+=2;
            ball = PhotonNetwork.Instantiate("Golf Ball",pos,transform.rotation);
            ball.layer = LayerMask.NameToLayer("Ball"+playerNumber);
            ball.GetComponent<Ball>().player = player;
            ball.GetComponent<Ball>().playerNumber = playerNumber;
            //ball.GetComponent<Ball>().setColor(playerNumber-1);
            beenGrabbed = true;
            player.GetComponent<MovementManager>().ball = ball;
            Debug.Log("GRABBED");
            }

        }
    }

    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.transform.tag == "Ball" && cooldown>.2f){
            player.GetComponent<MovementManager>().raiseScore();
            cooldown = 0;
        }
    }
}
