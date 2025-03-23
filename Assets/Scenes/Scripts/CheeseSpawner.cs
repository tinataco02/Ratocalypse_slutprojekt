using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;

public class CheeseSpawner : MonoBehaviour
{
    public GameObject cheesePrefab; 
    private ARRaycastManager raycastManager;  
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();  
    public float spawnInterval = 5f; 
    public float initialDelay = 3f; 
    public int maxCheese = 5;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        StartCoroutine(SpawnCheese());  
    }

IEnumerator SpawnCheese()
{
    yield return new WaitForSeconds(initialDelay); 

    while (true)
    {
        yield return new WaitForSeconds(spawnInterval);  

        
        int currentCheeseCount = GameObject.FindGameObjectsWithTag("Cheese").Length;
        
        if (currentCheeseCount >= maxCheese)
        {
            continue; 
        }

        
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        if (raycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            
            Vector3 spawnPosition = new Vector3(
                hitPose.position.x + Random.Range(-1f, 1f),
                hitPose.position.y,
                hitPose.position.z + Random.Range(-1f, 1f)
            );

            
            GameObject newCheese = Instantiate(cheesePrefab, spawnPosition, Quaternion.identity);
            newCheese.tag = "Cheese"; 
        }
    }
}

}
