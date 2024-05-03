using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerObject;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Vector3 spawnPos = new Vector3(45.2099991f,0.377978265f,18.2600002f);
        GameObject newPlayer = PhotonNetwork.Instantiate("NetworkPlayer",spawnPos,transform.rotation);
        newPlayer.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Player");
        int playerNumber = newPlayer.GetComponent<PhotonView>().Owner.ActorNumber;
        //GameObject ball = PhotonNetwork.Instantiate("Golf Ball",transform.position,transform.rotation);
        GameObject club = PhotonNetwork.Instantiate("Golf Club",transform.position,transform.rotation);
        club.GetComponent<Club>().playerNumber = playerNumber;
        club.GetComponent<Club>().player = newPlayer;
        //ball.layer = LayerMask.NameToLayer("Ball"+playerNumber);
        club.layer = LayerMask.NameToLayer("Club"+playerNumber);
        club.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Club"+playerNumber);
        club.GetComponent<XRGrabInteractable>().interactionLayers= InteractionLayerMask.GetMask("PlayerMask");;
        newPlayer.GetComponent<MovementManager>().club = club;
        spawnPos = new Vector3(45.2099991f,2.377978265f,16.2099991f);
        club.transform.position = spawnPos;
        club.transform.Rotate(0,90,0);
        
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerObject);
    }
}
