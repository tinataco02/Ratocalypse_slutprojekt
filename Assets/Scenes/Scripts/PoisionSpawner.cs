using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Collections;

public class PoisionSpawner : MonoBehaviour
{
    public GameObject poisionPrefab;  
    private ARRaycastManager raycastManager;  
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();  
    public float spawnInterval = 20f; 
    public float initialDelay = 5f; 
    public int maxPoision = 3;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        StartCoroutine(SpawnPoision());  
    }

    IEnumerator SpawnPoision()
    {
        yield return new WaitForSeconds(initialDelay); 

        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);  

            if (GameObject.FindGameObjectsWithTag("Poision").Length >= maxPoision)
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


                Instantiate(poisionPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
