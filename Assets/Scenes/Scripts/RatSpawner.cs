using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RatSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public float spawnInterval = 60f;
    public int maxRats = 5; 
    public float moveDistance = 0.015f;
    public float rotationAngle = 90f;
    public float eatDistance = 0.2f;
    public float growthFactor = 1.5f;
    public TextMeshProUGUI ratCountText; 
    public static int aliveRatCount = 0; 
    public TextMeshProUGUI killCountText; 
    public static int killedRatCount = 0; 
    public int cheeseEatenCount = 0;
    public int maxCheeseBeforeLose = 30;
    public TextMeshProUGUI gameOverText;
    private bool gameOver = false; 
    public TextMeshProUGUI cheeseEatenCountText;


    private bool isSpawning = false;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnRats());
        }
    }


    IEnumerator SpawnRats()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (gameOver) yield break; 

            
            if (GameObject.FindGameObjectsWithTag("Rat").Length >= maxRats)
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

                GameObject spawnedRat = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                spawnedRat.tag = "Rat";

                aliveRatCount++;

                UpdateRatCountUI();

                StartCoroutine(MoveRat(spawnedRat));
            }
        }
    }

    IEnumerator MoveRat(GameObject rat)
    {
        Animator ratAnimator = rat.GetComponent<Animator>(); 
        bool isFrozen = Random.value < 0.3f; 

        if (isFrozen && ratAnimator != null)
        {
            ratAnimator.enabled = false; 
            Debug.Log("🐀 Råttan frös!");
            yield return new WaitForSeconds(Random.Range(5f, 10f)); 
            isFrozen = false;
            ratAnimator.enabled = true; 
            Debug.Log("🏃‍♂️ Råttan börjar springa igen!");
        }

        while (rat != null && !gameOver)
        {
            yield return new WaitForSeconds(0.2f);

            if (rat != null && !isFrozen) 
            {
                GameObject cheese = FindClosestCheese(rat);
                if (cheese != null)
                {
                    Vector3 directionToCheese = (cheese.transform.position - rat.transform.position).normalized;
                    rat.transform.rotation = Quaternion.LookRotation(new Vector3(directionToCheese.x, 0, directionToCheese.z));
                    rat.transform.position += rat.transform.forward * moveDistance;

                    if (Vector3.Distance(rat.transform.position, cheese.transform.position) < eatDistance)
                    {
                        Destroy(cheese);
                        GrowRat(rat);
                        cheeseEatenCount++;
                        UpdateCheeseEatenCountUI();
                    }
                    if (cheeseEatenCount > maxCheeseBeforeLose && !gameOver)
                    {
                        Debug.Log("❌ GAME OVER! Råttan åt för mycket ost!");
                        ShowGameOver();
                        StartCoroutine(RestartGameWithDelay(6f));
                    }
                }
                else
                {
                    float randomRotation = Random.Range(-rotationAngle, rotationAngle);
                    rat.transform.Rotate(0, randomRotation, 0);
                    rat.transform.position += rat.transform.forward * moveDistance;
                }
            }
        }
    }

    void GrowRat(GameObject rat)
    {
        rat.transform.localScale *= (1.2f + growthFactor);
    }

    GameObject FindClosestCheese(GameObject rat)
    {
        GameObject[] cheeses = GameObject.FindGameObjectsWithTag("Cheese");
        if (cheeses.Length == 0) return null;

        return cheeses.OrderBy(c => Vector3.Distance(rat.transform.position, c.transform.position)).FirstOrDefault();
    }


    public void UpdateRatCountUI()
    {
        if (ratCountText != null)
        {
            ratCountText.text = $"{aliveRatCount}";
        }
    }

    public void UpdateKillCountUI()
{
    if (killCountText != null)
    {
        killCountText.text = $" {killedRatCount}"; 
    }
}


    public void UpdateCheeseEatenCountUI()
    {
        if (cheeseEatenCountText != null)
        {
            cheeseEatenCountText.text = $" {cheeseEatenCount}"; 
        }
    }

    void ShowGameOver()
    {
        gameOver = true;
        if (gameOverText != null)
        {
            gameOverText.text = "GAME OVER!";
        }
    }

    IEnumerator RestartGameWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
