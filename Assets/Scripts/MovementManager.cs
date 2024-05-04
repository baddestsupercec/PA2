using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Photon.Pun.UtilityScripts;
using UnityEditor;

public class MovementManager : MonoBehaviour
{
    private PhotonView myView;
    public GameObject myChild;
    public GameObject myChild2;

    private float xInput;
    private float yInput;
    private float movementSpeed = 6.0f;

    public InputData inputData;
    //[SerializeField] private GameObject myObjectToMove;
    private Rigidbody myRB;
    private Transform myXRRig;

    private GameObject cam;
    public Transform parentTransform;
    public GameObject mainCam;
    private Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    public GameObject myXrOrigin;

    public CameraGaze camGaze;
    public controllerGaze controllerGaze;
    public InputActionReference resetButton;
    public GameObject club;
    public GameObject ball;
    public GameObject[] courseScoreText = new GameObject[3];
    public GameObject overallScoreText;

    public int curCourse;
    public int overallScore;
    public int courseScore;
    public int[] courseScores = new int[3];
    public bool[] completedCourses = new bool[3];
    public int coursesComplete;
    public bool finishedFirst;
    public int otherPlayersComplete;
    public int howManyPlayers;
    public int winningPlayer;
    public int winningPlayerScore;
    public int[] PlayerScores = new int[4];
    public GameObject[] spawns = new GameObject[3];
    public GameObject[] clubSpawns = new GameObject[3];
    public List<GameObject> otherPlayers = new List<GameObject>();
    public int playerNumber;
    public bool gameOver;
    private bool justFinished;
    private float teleportCooldown;
    public GameObject hitSound;
    public GameObject scoreSound;

    // Start is called before the first frame update
    void Start()
    {
        myView = GetComponent<PhotonView>();

        myChild = transform.GetChild(0).gameObject;
        myRB = myChild.GetComponent<Rigidbody>();
        myChild2 = transform.GetChild(1).gameObject;

        myXrOrigin = GameObject.Find("XR Origin");
        scoreSound = GameObject.Find("Score");
        hitSound = GameObject.Find("Hit");
        spawns[0] = GameObject.Find("Course1Spawn");
        spawns[1] = GameObject.Find("Course2Spawn");
        spawns[2] = GameObject.Find("Course3Spawn");
        clubSpawns[0] = GameObject.Find("table1");
        clubSpawns[1] = GameObject.Find("table2");
        clubSpawns[2] = GameObject.Find("table3");
        courseScoreText[0] = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject;
        courseScoreText[1] = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
        courseScoreText[2] = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(3).gameObject;
        overallScoreText = myXrOrigin.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
        parentTransform = myXrOrigin.transform;
        cam = myXrOrigin.transform.GetChild(0).gameObject;
        mainCam = myXrOrigin.transform.GetChild(0).GetChild(0).gameObject;
        myXRRig = myXrOrigin.transform;
        inputData = myXrOrigin.GetComponent<InputData>();
        myRB.constraints = RigidbodyConstraints.FreezeRotation;
        camGaze = mainCam.GetComponent<CameraGaze>();
        controllerGaze =  myXrOrigin.transform.GetChild(0).GetChild(2).gameObject.GetComponent<controllerGaze>();
        playerNumber = GetComponent<PhotonView>().Owner.ActorNumber;
        winningPlayerScore = 10000;
        if(myView!=null){
            if(myView.IsMine){
                myChild2.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
        //resetButton.action.performed+=reset;
    }

    public void reset(){
        if(myView.IsMine){
            Debug.Log("RESET");
            //courseScoreText.GetComponent<TextMeshProUGUI>().text = "RESET";
            if(ball!=null){
                PhotonNetwork.Destroy(ball);
        }
        if(club!=null){
            club.GetComponent<Club>().beenGrabbed = false;
            Vector3 spawnPos = clubSpawns[curCourse].transform.position;
            spawnPos.y+=6;
            club.transform.position = spawnPos;
        }
        }
    }
    public void hovered(){
        if(myView!=null){
        if(myView.IsMine){
            Debug.Log("HOVERED");
            //courseScoreText.GetComponent<TextMeshProUGUI>().text = "HOVERED";
        }
        }
    }
    public bool spawnBall(){
        if(myView.IsMine){
            if(completedCourses[curCourse] == false){
                Debug.Log("SPAWN");
                //courseScoreText.GetComponent<TextMeshProUGUI>().text = "SPAWN";
                return true;
            }
        }
        return false;
    }

    public void raiseScore(){
        courseScores[curCourse]++;
        overallScore++;
        courseScoreText[curCourse].GetComponent<TextMeshProUGUI>().text = "Course " + (curCourse+1)+": " + courseScores[curCourse];
        overallScoreText.GetComponent<TextMeshProUGUI>().text = "Overall Score: " + overallScore;
        hitSound.GetComponent<AudioSource>().Play();
    }

    public void scored(){
        coursesComplete++;
        completedCourses[curCourse] = true;
        scoreSound.GetComponent<AudioSource>().Play();
        if(ball!=null)
            PhotonNetwork.Destroy(ball);
    }

    [PunRPC]
    void endGame()
    {
        if (myView.IsMine){
            gameOver = true;
            movementSpeed = 0;
        }
       
    }

    [PunRPC]
    void CompletedCourse(int[] score)
    {
        if (myView.IsMine){
                PlayerScores[score[1]] = score[0];
                otherPlayersComplete++;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (myView.IsMine)
        {
            if(ball!=null){
                if(Vector3.Distance(ball.transform.position,myChild.transform.position) > 100){
                    reset();
                }
            }
            if(Vector3.Distance(spawns[curCourse].transform.position,myChild.transform.position) > 100){
                    reset();
                     myRB.position = spawns[curCourse].transform.position;
            }
            teleportCooldown+=Time.deltaTime;
            if (Input.GetKeyDown("space"))
        {
            club.GetComponent<Club>().grabbed();
        }
            GameObject[] temp = GameObject.FindGameObjectsWithTag("Player");
            foreach(GameObject player in temp){
                if(player.transform.parent.gameObject != gameObject){
                    if(!otherPlayers.Contains(player.transform.parent.gameObject)){
                        otherPlayers.Add(player.transform.parent.gameObject);
                    }
                }
            }
        howManyPlayers = otherPlayers.Count+1;
        //overallScoreText.GetComponent<TextMeshProUGUI>().text = otherPlayersComplete.ToString()+" " + howManyPlayers + " " + gameOver + " " +completedCourses[curCourse];
        if(gameOver){
            for(int i = 0; i<howManyPlayers; i++){
                if(PlayerScores[i] < winningPlayerScore){
                    winningPlayerScore = PlayerScores[i];
                    winningPlayer = i+1;
                }
            }
            courseScoreText[1].GetComponent<TextMeshProUGUI>().text = "";
            courseScoreText[2].GetComponent<TextMeshProUGUI>().text = "";
            if(winningPlayer!=playerNumber)
                courseScoreText[0].GetComponent<TextMeshProUGUI>().text = "Player " + winningPlayer + " has won with score " + winningPlayerScore;
            else{
                courseScoreText[0].GetComponent<TextMeshProUGUI>().text = "You have won!";
            }
            overallScoreText.GetComponent<TextMeshProUGUI>().text = "You Finished with score " +overallScore;
        }
        else if(coursesComplete==3){
            overallScoreText.GetComponent<TextMeshProUGUI>().text = "You Finished with score " +overallScore;
            if(!justFinished){
                otherPlayersComplete++;
                justFinished = true;
                int[] myScore = new int[2];
                myScore[0] = overallScore;
                myScore[1] = playerNumber-1;
                PlayerScores[playerNumber-1] = overallScore;
                foreach(GameObject player in otherPlayers){
                    player.GetComponent<PhotonView>().RPC("CompletedCourse",player.GetComponent<PhotonView>().Controller,myScore);
        }
            }
        }
        if(otherPlayersComplete==howManyPlayers){
            endGame();
        }
        if (Input.GetKeyDown("a"))
        {
            reset();
        }
        if (Input.GetKeyDown("b"))
        {
                    curCourse++;
                    curCourse = curCourse%3;
                    myRB.position = spawns[curCourse].transform.position;
                    courseScore = 0; 
                    reset();
        }
            parentTransform.transform.position = myChild.transform.position;

            if (inputData.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 movement))
            {
                //Debug.Log("MOVE");
                xInput = movement.x;
                yInput = movement.y;
                Vector3 moveDir = new Vector3(xInput, 0, yInput).normalized;
                Vector3 targetMoveAmount = moveDir * movementSpeed;
                moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
            }
            bool trigger = false;
            if (inputData.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out trigger))
            {
                if (trigger && camGaze.canTeleport && teleportCooldown>1)
                {
                    teleportCooldown = 0;
                    curCourse = camGaze.tpSign.GetComponent<CourseTeleport>().courseNumber-1;
                    myRB.position = spawns[curCourse].transform.position;
                    courseScore = 0; 
                    reset();
                }
            }
            trigger = false;
            if (inputData.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out trigger))
            {
                if (trigger && teleportCooldown>1)
                {
                    teleportCooldown = 0;
                    myRB.position = spawns[curCourse].transform.position;
                    courseScore = 0; 
                    reset();
                }
            }
             trigger = false;
            if (inputData.leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out trigger))
            {
                if (trigger && teleportCooldown>1)
                {
                    teleportCooldown = 0;
                    myRB.position = spawns[curCourse].transform.position;
                    courseScore = 0; 
                    reset();
                }
            }
            if (inputData.rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out trigger))
            {
                if (trigger && controllerGaze.canSelect)
                {
                    reset();
                }
            }
            if (inputData.leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out trigger))
            {
                if (trigger && camGaze.canTeleport && teleportCooldown>1)
                {
                    teleportCooldown = 0;
                    curCourse = camGaze.tpSign.GetComponent<CourseTeleport>().courseNumber-1;
                    myRB.position = spawns[curCourse].transform.position;
                    courseScore = 0;
                    reset();
                }
            }
            if (inputData.leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out trigger))
            {
                if (trigger && controllerGaze.canSelect)
                {
                    reset();
                }
            }
            //parentTransform.rotation = Quaternion.Euler(new Vector3(myChild.transform.rotation.x, parent.transform.rotation.y, myChild.transform.rotation.z));
        }
    }

    private void FixedUpdate()
    {
        //myRB.AddForce(xInput * movementSpeed, 0, yInput * movementSpeed);
        if (myView.IsMine)
        {
            //Debug.Log("MOVING");
            Vector3 localMove = mainCam.transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
            // Rigidbody rb = GetComponent<Rigidbody>();
            myRB.MovePosition(myRB.position + localMove);
            myChild2.transform.position = myChild.transform.position;
            //ui.text = myChild.transform.rotation.eulerAngles.ToString();
            cam.transform.rotation = myChild.transform.rotation;
            Vector3 rot = mainCam.transform.rotation.eulerAngles;
            rot.x = myChild2.transform.rotation.x;
            rot.z = myChild2.transform.rotation.z;
            myChild2.transform.eulerAngles = rot;
        }
    }
}