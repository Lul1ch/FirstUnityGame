using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    private CubePos curCube = new CubePos(0,1,0);
    private float cubeReplacingSpeed = 1f;
    public Transform cubeToPlace;
    private Rigidbody allCubesRb;
    private bool isLose = false, firstCube = false;

    public GameObject[] canvasMenu;
    public List<GameObject> customCubes = new List<GameObject>{};
    private int boardOfScore = 0;

    public List<Color> backgroundColors = new List<Color>{};
    private Color newBGColor, prevBGColor;

    private float prevCameraYPos;
    private int prevCameraHorPos;
    private Transform mainCam;
    private float cameraStartPosY;
    private float moveCameraOnYScale;
    public float camMoveSpeed = 5f;
    
    public GameObject cubeToCreate, allCubes, vfx;

    public TMP_Text scoreText;

    private List<Vector3> allCubesPositions = new List<Vector3>{
        new Vector3(0,1,0),
    };

    private void Start() {
        mainCam = Camera.main.transform;
        cameraStartPosY = mainCam.localPosition.y;
        prevCameraYPos = cameraStartPosY;
        newBGColor = Camera.main.backgroundColor;

        scoreText.text = "<size=50>Best: " + PlayerPrefs.GetInt("score") + "</size> \n <size=35>Now: </size> 0";

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        StartCoroutine(ShowCubePlace());
    }

    private void Update() {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && !EventSystem.current.IsPointerOverGameObject()){
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began){
                return;
            }
#endif      
            if(!firstCube){
                firstCube = true;
                foreach(GameObject elem in canvasMenu ){
                    Destroy(elem);
                }
            }
            GameObject newCube = Instantiate(
                customCubes[UnityEngine.Random.Range(0, boardOfScore)],
                cubeToPlace.position,
                Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);
            curCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(curCube.getVector());

            if(PlayerPrefs.GetString("music") != "No"){
                GetComponent<AudioSource>().Play();
            }
            GameObject vfxToDestroy = Instantiate(vfx, newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(vfxToDestroy, 1.5f);

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPositions();
            CameraController();

        }

        if (!isLose && allCubesRb.velocity.magnitude > 0.1f){
            Destroy(cubeToPlace.gameObject);
            isLose = true;
        }

        if (allCubesPositions.Count != 1) {
            mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, 
            new Vector3(mainCam.localPosition.x, moveCameraOnYScale, mainCam.localPosition.z), 
            camMoveSpeed *Time.deltaTime);
        }

        if (Camera.main.backgroundColor != newBGColor){
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, newBGColor, Time.deltaTime / 1.5f);
        }
    }

    IEnumerator ShowCubePlace(){
        while(!isLose){
            SpawnPositions();

            yield return new WaitForSeconds(cubeReplacingSpeed);
        }
    }

    private void SpawnPositions(){
        List<Vector3> positions = new List<Vector3>();
        List<int> difses = new List<int>{1, 0 , 0, };
        int index = 0;
        for(int i =0; i < 5;++i){
            if (IsPositionEmpty(new Vector3(curCube.x + difses[0], curCube.y + difses[1],curCube.z + difses[2]))){
                if (curCube.x + difses[0] != cubeToPlace.position.x || curCube.y + difses[1] != cubeToPlace.position.y || curCube.z + difses[2] != cubeToPlace.position.z){
                    positions.Add(new Vector3(curCube.x + difses[0], curCube.y + difses[1],curCube.z + difses[2]));
                }
            }
            difses[changeI(index)] = -1 * difses[index]; 
            difses[index] = 0;
            index = changeI(index);
        }
        if (positions.Count == 1){
            cubeToPlace.position = positions[0];
        } else if (positions.Count == 0){
            isLose = true;
        } else {
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        }
    }

    private bool IsPositionEmpty(Vector3 targetPos){
        if(targetPos.y == 0){
            return false;
        }

        foreach(Vector3 pos in allCubesPositions){
            if (Equals(targetPos, pos)){
                return false;
            }
        }
        return true;
    }

    int changeI(int index){
        ++index;
        if(index > 2){
            index = 0;
        }
        return index;
    }

    private void CameraController(){
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach(Vector3 pos in allCubesPositions){
            if (Mathf.Abs(Convert.ToInt32(pos.x)) >  Mathf.Abs(maxX)){
                maxX = Convert.ToInt32(pos.x);
            }

            if (Mathf.Abs(Convert.ToInt32(pos.y)) >  Mathf.Abs(maxY)){
                maxY = Convert.ToInt32(pos.y);
            }

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > Mathf.Abs(maxZ)){
                maxZ = Convert.ToInt32(pos.z);
            }

            moveCameraOnYScale = cameraStartPosY + curCube.y - 1f;

            maxHor = (maxX > maxZ) ? maxX : maxZ;

            if (prevCameraHorPos <= maxHor - 3){
                mainCam.localPosition -= new Vector3(0, 0, 2.5f);
                prevCameraHorPos = maxHor;
            }

            scoreText.text = "<size=50>Best: " + PlayerPrefs.GetInt("score") + "</size> \n <size=35>Now: </size> " + maxY;

            //Debug.Log("Board " + boardOfScore + "Score " + (PlayerPrefs.GetInt("score")));
            
            if (PlayerPrefs.GetInt("score") < maxY){
                PlayerPrefs.SetInt("score", maxY);
            }
            if (PlayerPrefs.GetInt("score") <= 70){  //WARNING!!! Ссылка на конкретное число, переделать в случае изменений
                if (PlayerPrefs.GetInt("score") >= 40){
                    boardOfScore = PlayerPrefs.GetInt("score") / 10 + 3;
                } else {
                    boardOfScore = PlayerPrefs.GetInt("score") / 5;
                }
            } else {
                boardOfScore = 10;
            }
            if (Convert.ToInt32(moveCameraOnYScale - cameraStartPosY) % 5 == 0 && prevCameraYPos != moveCameraOnYScale){
                prevBGColor = newBGColor;
                while(prevBGColor == newBGColor){
                    newBGColor = backgroundColors[UnityEngine.Random.Range(0, backgroundColors.Count)];
                }
                prevCameraYPos = moveCameraOnYScale;
            }

        }
    }
}

struct CubePos {
    public int x, y, z;
    public CubePos(int x, int y,int z){
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector(){
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos){
        this.x = Convert.ToInt32(pos.x);
        this.y = Convert.ToInt32(pos.y);
        this.z = Convert.ToInt32(pos.z);
    }
}
