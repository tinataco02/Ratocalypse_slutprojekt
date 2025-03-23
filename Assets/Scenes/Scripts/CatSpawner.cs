using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;

public class CatSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public float spawnInterval = 60f;
    public int maxCats = 1;
    public float moveDistance = 0.015f;
    public float rotationAngle = 90f;
    public GameObject catPrefab;
    public Transform spawnPoint;
    public Camera arCamera;

    private bool isSpawning = false;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnCats());
        }
    }

    IEnumerator SpawnCats()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (GameObject.FindGameObjectsWithTag("Cat").Length >= maxCats)
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

                GameObject spawnedCat = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                spawnedCat.tag = "Cat";

                Vector3 lookDirection = arCamera.transform.position - spawnedCat.transform.position;
                lookDirection.y = 0;
                spawnedCat.transform.rotation = Quaternion.LookRotation(lookDirection);

                StartCoroutine(MoveCat(spawnedCat));
            }
        }
    }

    IEnumerator MoveCat(GameObject cat)
    {
        yield return new WaitForSeconds(6.2f);

        while (cat != null)
        {
            yield return new WaitForSeconds(0.2f);

            float moveSpeed = 0.1f;

            if (Random.value < 0.3f)
            {
                float turnAngle = Random.Range(-30f, 30f);
                cat.transform.Rotate(0, turnAngle, 0);
            }

            Vector3 forward = new Vector3(cat.transform.forward.x, 0, cat.transform.forward.z).normalized;
            cat.transform.position += forward * moveSpeed;
        }
    }

}
