using UnityEngine;

public class RatDestroyer : MonoBehaviour
{
    private RaycastOnTrackedImage raycastOnTrackedImage;

    void Start()
    {
        raycastOnTrackedImage = FindAnyObjectByType<RaycastOnTrackedImage>();
    }

    public void DieOnRaycastHit(RaycastHit hit, string usedItem)
    {
        Debug.Log($"Råtta dödas med: {usedItem}");

        switch (usedItem)
        {
            case "Poision":
                Debug.Log("Gift användes!");
                break;
            case "Trap":
                Debug.Log("Fälla användes!");
                break;
            case "Cat":
                Debug.Log("Katt användes!");
                break;
        }

        Handheld.Vibrate();
        Destroy(gameObject);

        RatSpawner.aliveRatCount--;
        RatSpawner.killedRatCount++;

        RatSpawner spawner = FindObjectOfType<RatSpawner>();
        if (spawner != null)
        {
            spawner.UpdateRatCountUI();
            raycastOnTrackedImage.ShowDeathMessage();
            spawner.UpdateKillCountUI();
        }
    }
}
