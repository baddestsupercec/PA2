using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
public class CollisionDetection : MonoBehaviour
{
    public GameObject player;
    public GameObject bear;
    public bool chaser = false;
    public int lives = 3;

    private PhotonView myView;

    private GameObject myBody;
    private GameObject myParent;
    private Rigidbody myRB;

    private GameObject bearMesh;
    private GameObject runnerMesh;

    public int playerNumber;


    private GameObject myXrOrigin;
    private GameObject heart1;
    private GameObject heart2;
    private GameObject heart3;

    private TextMeshProUGUI time;

    private TextMeshProUGUI role;

    private TextMeshProUGUI TP;
    private GameObject canvas;
    private GameObject timeObject;

    private GameObject roleObject;
    private int localPlayerIndex = -1;

    private float chasingTime = 0;

    private bool chaserAssigned = false;
    private bool gameOver = false;

    private bool repelbool;
    private bool attractbool;

    private Vector3 repelPoint;
    private Vector3 attractPoint;

    private float repelTime;
    private float attractTime;

    public GameObject otherPlayer;
    

    // Start is called before the first frame update
    void Start()
    {
        myXrOrigin = GameObject.Find("XR Origin");
        heart1 = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        heart2 = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
        heart3 = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;

        //myParent = transform.parent.gameObject;
        bear.SetActive(false);
        player.SetActive(false);

        myView = GetComponent<PhotonView>();
        myBody = transform.GetChild(0).gameObject;
        myRB = myBody.GetComponent<Rigidbody>();

        if (myView.IsMine)
        {
            heart1.GetComponent<SpriteRenderer>().enabled = true;
            heart2.GetComponent<SpriteRenderer>().enabled = true;
            heart3.GetComponent<SpriteRenderer>().enabled = true;
        }

        canvas = GameObject.Find("Canvas");
        timeObject = canvas.transform.GetChild(0).gameObject;
        time = timeObject.GetComponent<TextMeshProUGUI>();
        roleObject = canvas.transform.GetChild(1).gameObject;
        role = roleObject.GetComponent<TextMeshProUGUI>();

        playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.Log("Start " + playerNumber + " " + gameObject.name);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Length: "+players.Length);
        if(playerNumber==1){
        if (players.Length == 1)
        {
            bear.SetActive(true);
            player.SetActive(false);
            chaser = true;
        } 
        else{
            bear.SetActive(false);
            player.SetActive(true);
            chaser = false;
        
        }
        }

        if(playerNumber==2){
        if (myView.IsMine)
        {
            bear.SetActive(false);
            player.SetActive(true);
            chaser = false;
        } 
        else{
            bear.SetActive(true);
            player.SetActive(false);
            chaser = true;
        
        }
        }

        if (myView != null)
        {
            if (myView.IsMine)
        {
            Debug.Log("Initialize  " + playerNumber);
            bearMesh = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).gameObject;
            bearMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;

            for (int i = 0; i <= 6; i++)
            {
                runnerMesh = transform.GetChild(1).GetChild(1).GetChild(i).gameObject;
                runnerMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        }



    }

    // Update is called once per frame

    [PunRPC]
    public void useForceAway(Vector3 pos){
        if(myView.IsMine){
            Debug.Log("Call REPEL");
            myView.RPC("repelOther", RpcTarget.Others, pos);
        }
    }
    [PunRPC]
    public void useForceTowards(Vector3 pos){
        if(myView.IsMine){
            Debug.Log("Call ATTRACT");
            myView.RPC("attractOther", RpcTarget.Others, pos);
        }
    }

    [PunRPC]
    public void repelOther(Vector3 pos){
        Debug.Log("TRY RECIEVE REPEL " + chaser);
        if(!myView.IsMine){
            otherPlayer.GetComponent<CollisionDetection>().repel(pos);
        }
    }

    public void repel(Vector3 pos){
            if(myView.IsMine){
            repelbool = true;
            repelTime = 0;
            repelPoint = pos;
            }
    }

    [PunRPC]
    public void attractOther(Vector3 pos){
        Debug.Log("TRY RECIEVE ATTRACT " + chaser);
        if(!myView.IsMine){
            otherPlayer.GetComponent<CollisionDetection>().attract(pos);
        }
    }
    
    public void attract(Vector3 pos){
        if(myView.IsMine){
            attractbool = true;
            attractTime = 0;
            attractPoint = pos;
            }
    }
    void Update()
    {
        if(otherPlayer == null){
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");

            foreach(GameObject player in temp){
                if(player.transform.parent.gameObject != gameObject){
                    otherPlayer = player.transform.parent.gameObject;
                }
            }
        }

        if (myView.IsMine)
        {
            if (chaser && !gameOver)
                {
                    timeObject.SetActive(true);
                    chasingTime += Time.deltaTime;
                    time.text = chasingTime.ToString("f0") + "s";
                    role.text = "CHASER";
                    /*if ((chasingTime % 1) == 0)
                    {
                        time.text = chasingTime.ToString();
                    }*/
                }
            else
                {
                    timeObject.SetActive(false);
                    if(!gameOver)
                        role.text = "RUNNER";
                        timeObject.SetActive(true);
                        time.text = "RUN!!!";
                        time.color = Color.black;
                }

                if (chaser && (chasingTime > 110))
        {
                    if (((int)chasingTime % 2) == 0)
                    {
                        time.color = Color.red;
                    }
                    else
                    {
                        time.color = Color.black;
                    }
                }
                else
                {
                    time.color = Color.red;
                }

                if (chasingTime > 120)
                {
                    chasingTime = 0;
                    lives--;
                    if (lives == 2)
                    {
                        heart3.SetActive(false);
                    }
                    else if (lives == 1)
                    {
                        heart2.SetActive(false);
                    }
                    else
                    {
                        heart1.SetActive(false);
                        endGameSequence();
                    }
                }


                if(repelbool){
                    repelPoint = otherPlayer.transform.GetChild(0).transform.position;
                    repelTime+=Time.deltaTime;
                    Debug.Log("REPEL " + repelTime + " " + myView.IsMine + " " + repelPoint);
                    myRB.AddForce((myBody.transform.position - repelPoint).normalized * 8);
                    if(repelTime>=5){
                        repelbool = false;
                    }
                }

                if(attractbool){
                    attractPoint = otherPlayer.transform.GetChild(0).transform.position;
                    attractTime+=Time.deltaTime;
                    Debug.Log("ATTRACT " + attractTime + " " + myView.IsMine + " " + attractPoint);
                    myRB.AddForce((attractPoint - myBody.transform.position).normalized * 8);
                    if(attractTime>=5){
                        attractbool = false;
                    }
                }
        }
    }

    public void collided()
    {
        Debug.Log("Collision");
       // if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player");
            Vector3 newPosition = gameObject.transform.position;


            if (!chaser)
            {
                chaser = true;
                chasingTime = 0;
                SwitchToBear(newPosition);
                lives--;
                if (myView.IsMine)
                {
                    if (lives == 2)
                    {
                        heart3.SetActive(false);
                    }
                    else if (lives == 1)
                    {
                        heart2.SetActive(false);
                    }
                    else
                    {
                        heart1.SetActive(false);
                        endGameSequence();
                    }
                    StartCoroutine(wait());
                }
            }
            else
            {
                chaser = false;
                chasingTime = 0;
                SwitchToPlayer(newPosition);
            }
            if (myView != null)
            {
                myView.RPC("changeCostume", RpcTarget.Others, chaser);
            }
          
           
        }

        Debug.Log(lives);
    }

    public void SwitchToBear(Vector3 position)
    {
        Debug.Log("Bear");
        player.SetActive(false);
        bear.SetActive(true);
        //bear.transform.position = position;
    }

    public void SwitchToPlayer(Vector3 position)
    {
        Debug.Log("PlayerCostume");
        bear.SetActive(false);
        player.SetActive(true);
        //player.transform.position = position;
    }

    void endGameSequence()
    {
        role.text = "GAME OVER";
        gameOver = true;
        timeObject.SetActive(false);
    }

    [PunRPC]
    void changeCostume(bool chaser)
    {
        if (myView.IsMine)
        {
            bearMesh = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).gameObject;
            bearMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;

            for(int i = 0; i <= 6; i++)
            {
                runnerMesh = transform.GetChild(1).GetChild(1).GetChild(i).gameObject;
                runnerMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
    }

    [PunRPC]
    void initializeCostume()
    {
        if (myView.IsMine)
        {
            Debug.Log("Initialize  " + playerNumber);
            bearMesh = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).gameObject;
            bearMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;

            for (int i = 0; i <= 6; i++)
            {
                runnerMesh = transform.GetChild(1).GetChild(1).GetChild(i).gameObject;
                runnerMesh.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
    }
    private IEnumerator wait()
    {
        yield return new WaitForSeconds(.3f);
        Vector3 position = Random.onUnitSphere * 51;
        myBody.transform.position = position;
    }
}
