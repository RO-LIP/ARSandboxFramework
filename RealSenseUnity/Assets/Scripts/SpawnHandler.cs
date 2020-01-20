using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    public GameObject housePrefab;
    public GameObject tigerPrefab;
    private Vector3 screenBounds;
    
    void OnEnable ()
    {
        EventManager.StartListening ("spawnTiger", spawnTiger);
        EventManager.StartListening ("spawnHouse", spawnHouse);
    }

    void OnDisable ()
    {
        EventManager.StopListening ("spawnTiger", spawnTiger);
        EventManager.StopListening ("spawnHouse", spawnHouse);
    }


    void spawnTiger(){
        GameObject a = Instantiate(tigerPrefab) as GameObject;
        a.transform.position= new Vector3(Random.Range(300,screenBounds.x), 100,Random.Range(4,220));
    }

    void spawnHouse(){
        GameObject a = Instantiate(housePrefab) as GameObject;
        // these "magic" numbers are just the spawning area ...
        // feel free to change this in a smarter way
        a.transform.position= new Vector3(Random.Range(300,screenBounds.x), 100,Random.Range(4,220));
    }

    // Start is called before the first frame update
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
}
