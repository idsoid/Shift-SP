using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] UIManager uiManager;
    [SerializeField] InputController inputController;
    [SerializeField] PlayerController playerController;
    [SerializeField] CameraController cameraController;
    [SerializeField] Transform worldTransform, worldRotator, worldMainParent;
    [SerializeField] List<Transform> wallList = new(); //Order: Front, Back, Left, Right, Top, Bottom
    bool worldRot = false;
    Vector3 worldRotTarget = Vector3.zero;
    float spawnTimer = 0f;
    float difficultyScaler = 30f;
    [SerializeField] Transform enemyPoolParent;
    [SerializeField] List<GameObject> enemyObjPool = new();
    [SerializeField] GameObject enemyPrefab;
    
    // Update is called once per frame
    void Update()
    {
        //Spawning of enemies
        spawnTimer -= Time.deltaTime;
        difficultyScaler -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            bool poolEmpty = true;
            spawnTimer = 5.0f;
            foreach (var enemy in enemyObjPool)
            {
                if (!enemy.activeSelf)
                {
                    enemy.transform.position = new Vector3(Random.Range(-20.0f, 20.0f), -59f, Random.Range(-20.0f, 20.0f));
                    enemy.SetActive(true);
                    poolEmpty = false;
                    break;
                }
            }
            if (poolEmpty)
            {
                GameObject enemyObj = Instantiate(enemyPrefab);
                enemyObj.transform.position = new Vector3(Random.Range(-20.0f, 20.0f), -59f, Random.Range(-20.0f, 20.0f));
            }
        }
        //Scaling of enemies
        if (difficultyScaler <= 0)
        {
            difficultyScaler = 30f;
            Debug.Log("enemy buff incoming");
            foreach (var enemy in enemyObjPool)
            {
                enemy.GetComponent<TestEnemy>().EnemyMaxHealth += 10.0f;
                enemy.GetComponent<TestEnemy>().EnemyMaxDamage += 5.0f;
            }
        }

        //World Rotating
        if (worldRot)
        {
            worldRotator.rotation = Quaternion.Lerp(worldRotator.rotation, Quaternion.Euler(worldRotTarget), 2.5f * Time.deltaTime);
            if (worldRotator.rotation == Quaternion.Euler(worldRotTarget) || 
                worldRotTarget.x == 180 && Mathf.Abs(worldRotator.rotation.x) == Quaternion.Euler(worldRotTarget).x)
            {
                worldRotator.rotation = Quaternion.Euler(worldRotTarget);
                worldRot = false;
                worldRotTarget = Vector3.zero;
                worldTransform.SetParent(worldMainParent);
                worldRotator.eulerAngles = Vector3.zero;
            }
        }

        //Player
        if (inputController.TryGetMovementAxisInput(out MovementAxisCommand move))
        {
            playerController.ReadMovementAxisCommand(move);
        }
        if (inputController.TryGetAttackButtonInput(out AttackButtonCommand attack))
        {
            playerController.ReadAttackActionCommand(attack);
        }
        if (inputController.TryGetMovementButtonInput(out MovementButtonCommand moveButton))
        {
            playerController.ReadMovementButtonCommand(moveButton);
        }
        if (inputController.TryGetShiftButtonInput(out ShiftButtonCommand shiftButton))
        {
            playerController.ReadShiftButtonCommand(shiftButton);
        }

        //Camera
        if (inputController.TryGetMouseAxisInput(out MouseAxisCommand cam))
        {
            cameraController.ReadMouseAxisCommand(cam);
        }

        //Update Controllers/Managers
        playerController.UpdatePlayer();
        cameraController.UpdateCamera();
    }

    public void UpdateWorld(string wallTag)
    {
        if (worldRot)
        {
            return;
        }

        worldTransform.SetParent(worldRotator);
        List<Transform> newList = new();
        switch (wallTag) //Order: Front, Back, Left, Right, Top, Bottom
        {
            case "Front": //Rotate 90 on x-axis
                worldRotTarget = new Vector3(90, 0, 0);
                //Reassigning
                #region
                //Top -> Front
                wallList[4].gameObject.tag = "Front";
                newList.Add(wallList[4]);
                //Bottom -> Back (Old Ground)
                wallList[5].gameObject.tag = "Back";
                wallList[5].gameObject.layer = LayerMask.NameToLayer("Wall");
                newList.Add(wallList[5]);
                //Left -> Left
                newList.Add(wallList[2]);
                //Right -> Right
                newList.Add(wallList[3]);
                //Back -> Top
                wallList[1].gameObject.tag = "Top";
                newList.Add(wallList[1]);
                //Front -> Bottom (New Ground)
                wallList[0].gameObject.tag = "Bottom";
                wallList[0].gameObject.layer = LayerMask.NameToLayer("Ground");
                newList.Add(wallList[0]);
                #endregion
                break;
            case "Back": //Rotate -90 on x-axis
                worldRotTarget = new Vector3(-90, 0, 0);
                //Reassigning
                #region
                //Bottom -> Front (Old Ground)
                wallList[5].gameObject.tag = "Front";
                wallList[5].gameObject.layer = LayerMask.NameToLayer("Wall");
                newList.Add(wallList[5]);
                //Top -> Back
                wallList[4].gameObject.tag = "Back";
                newList.Add(wallList[4]);
                //Left -> Left
                newList.Add(wallList[2]);
                //Right -> Right
                newList.Add(wallList[3]);
                //Front -> Top
                wallList[0].gameObject.tag = "Top";
                newList.Add(wallList[0]);
                //Back -> Bottom (New Ground)
                wallList[1].gameObject.tag = "Bottom";
                wallList[1].gameObject.layer = LayerMask.NameToLayer("Ground");
                newList.Add(wallList[1]);
                #endregion
                break;
            case "Left": //Rotate 90 on z-axis
                worldRotTarget = new Vector3(0, 0, 90);
                //Reassigning
                #region
                //Front -> Front
                newList.Add(wallList[0]);
                //Back -> Back
                newList.Add(wallList[1]);
                //Top -> Left
                wallList[4].gameObject.tag = "Left";
                newList.Add(wallList[4]);
                //Bottom -> Right (Old Ground)
                wallList[5].gameObject.tag = "Right";
                wallList[5].gameObject.layer = LayerMask.NameToLayer("Wall");
                newList.Add(wallList[5]);
                //Right -> Top
                wallList[3].gameObject.tag = "Top";
                newList.Add(wallList[3]);
                //Left -> Bottom (New Ground)
                wallList[2].gameObject.tag = "Bottom";
                wallList[2].gameObject.layer = LayerMask.NameToLayer("Ground");
                newList.Add(wallList[2]);
                #endregion
                break;
            case "Right": //Rotate -90 on z-axis
                worldRotTarget = new Vector3(0, 0, -90);
                //Reassigning
                #region
                //Front -> Front
                newList.Add(wallList[0]);
                //Back -> Back
                newList.Add(wallList[1]);
                //Bottom -> Left (Old Ground)
                wallList[5].gameObject.tag = "Left";
                wallList[5].gameObject.layer = LayerMask.NameToLayer("Wall");
                newList.Add(wallList[5]);
                //Top -> Right 
                wallList[4].gameObject.tag = "Right";
                newList.Add(wallList[4]);
                //Left -> Top
                wallList[2].gameObject.tag = "Top";
                newList.Add(wallList[2]);
                //Right -> Bottom (New Ground)
                wallList[3].gameObject.tag = "Bottom";
                wallList[3].gameObject.layer = LayerMask.NameToLayer("Ground");
                newList.Add(wallList[3]);
                #endregion
                break;
            case "Top": //Rotate 180 on x-axis
                worldRotTarget = new Vector3(180, 0, 0);
                //Reassigning
                #region
                //Back -> Front
                wallList[1].gameObject.tag = "Front";
                newList.Add(wallList[1]);
                //Front -> Back
                wallList[0].gameObject.tag = "Back";
                newList.Add(wallList[0]);
                //Left -> Left
                newList.Add(wallList[2]);
                //Right -> Right
                newList.Add(wallList[3]);
                //Bottom -> Top (Old Ground)
                wallList[5].gameObject.tag = "Top";
                wallList[5].gameObject.layer = LayerMask.NameToLayer("Wall");
                newList.Add(wallList[5]);
                //Top -> Bottom (New Ground)
                wallList[4].gameObject.tag = "Bottom";
                wallList[4].gameObject.layer = LayerMask.NameToLayer("Ground");
                newList.Add(wallList[4]);
                #endregion
                break;
            default:
                break;
        }
        wallList.Clear();
        foreach (var wall in newList)
        {
            wallList.Add(wall);
        }
        worldRot = true;
    }
    public GameObject GetPlayer()
    {
        return playerController.gameObject;
    }
    public Transform GetEnemyPool()
    {
        return enemyPoolParent;
    }
    public void AddToEnemyPool(GameObject enemy)
    {
        enemyObjPool.Add(enemy);
    }
}
