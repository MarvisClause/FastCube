using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaseSpeed = 0.5f;
    public Transform cubeToPlace;
    public float camMoveToYPosition,camMoveSpeed=2f;

    public Text scoreText;

    public GameObject cubeToCreate, allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb; 

    public Color[] bgColors;
    private Color toCameraColor;

    private bool IsLose, firstCube;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
    new Vector3(0,0,0),
    new Vector3(1,0,0),
    new Vector3(-1,0,0),
    new Vector3(0,1,0),
    new Vector3(0,0,1),
    new Vector3(0,0,-1),
    new Vector3(1,0,1),
    new Vector3(-1,0,-1),
    new Vector3(-1,0,1),
    new Vector3(1,0,-1) };

    private int prevCoutMaxHorizontal;
    private Coroutine showCubePlace;
    private Transform mainCam; 

    private void Start()
    {
        scoreText.text = "<size=22><color=#E06156>Best score:</color></size>" + PlayerPrefs.GetInt("score") + " \n<size=22> now:</size> 0";

        toCameraColor =Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + nowCube.y - 1f; 

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
        
    } 
     
    private void Update()
    {
         if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace !=null && allCubes!=null &&!EventSystem.current.IsPointerOverGameObject())
        {
            #if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
            #endif

            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }
            
            GameObject newCube = Instantiate(
                cubeToCreate,
                cubeToPlace.position,
                Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.getVector());

            Instantiate(vfx,cubeToPlace.position,Quaternion.identity);

            allCubesRb.isKinematic = true; 
            allCubesRb.isKinematic = false;
            
            SpawnPositions();
            MoveCameraChangeBg();
        } 
          
         if(!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            Destroy(cubeToPlace.gameObject);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
            new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z), 
            camMoveSpeed*Time.deltaTime);  

        // For slow change color bg
        if (Camera.main.backgroundColor != toCameraColor) 
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
    }

    IEnumerator ShowCubePlace()
    {
        while(true) 
        {
            SpawnPositions();

            yield return new WaitForSeconds (cubeChangePlaseSpeed);
        } 
    } 
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();  
        if(IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
        positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)); 
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x , nowCube.y -1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
             positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
             positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    } 
    private bool IsPositionEmpty(Vector3 targetPos)
    {
    if (targetPos.y == 0)
        return false;  

    foreach(Vector3 pos in allCubesPositions)
    {
        if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
            return false;
    }
        return true;
    } 
    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor; 

        // ALGORITHM FOR OBTAINING THE MAXIMUM ELEMENT 
        foreach (Vector3 pos in allCubesPositions)  
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x))>maxX)
                maxX = Convert.ToInt32(pos.x);
            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY)
                maxY = Convert.ToInt32(pos.y);
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }
        maxY--; 

        // Save score
        if (PlayerPrefs.GetInt("score") < maxY)
        {
            PlayerPrefs.SetInt("score", maxY);
        }


        scoreText.text = "<size=22><color=#E06156>Best score:</color></size>" + PlayerPrefs.GetInt("score") + " \n<size=22> now:</size>"+maxY;

        camMoveToYPosition = 5.9f + nowCube.y - 1f; 

        // Ternary operation
        maxHor = maxX > maxZ ? maxX : maxZ;    

        if (maxHor %3 == 0 && prevCoutMaxHorizontal != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, -2.5f);
            prevCoutMaxHorizontal = maxHor;
        }

        if (maxY >= 30)
            toCameraColor = bgColors[2]; 
        else if (maxY >= 20)
            toCameraColor = bgColors[1];
        else if (maxY >= 10)
            toCameraColor = bgColors[0];

    }
} 
struct CubePos
{
    public int x, y, z; 
    public CubePos(int x, int y,int z)
    {
        this.x = x;
        this.y = y; 
        this.z = z;
    } 
    public Vector3 getVector()
    {
        return new Vector3(x, y, z); 
    }
    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}
