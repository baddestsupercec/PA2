using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public int playerNumber;
    public Material[] materials;
    void Start()
    {
        playerNumber = GetComponent<PhotonView>().Owner.ActorNumber;
        setColor(playerNumber-1);
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void hovered(){
        Debug.Log("HOVERED");
    }

    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.transform.tag == "Goal"){
            player.GetComponent<MovementManager>().scored();
        }
    }
    public void setColor(int color){
        GetComponent<MeshRenderer>().material = materials[color];
    }
}
