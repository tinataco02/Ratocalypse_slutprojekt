using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.XR.ARSubsystems;

public class RaycastOnTrackedImage : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject objectPrefab;
    private GameObject currentObject;
    public float interactionDistance = 0.5f;
    public Camera arCamera;

    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI deathMessageText;

    private InventoryManager inventoryManager;
    private LineRenderer lineRenderer;

    public GameObject bloodEffectPrefab;
    public AudioSource audioSource;
    public AudioClip ratDeathSound;

    void Start()
    {
        if (imageManager == null)
            imageManager = FindObjectOfType<ARTrackedImageManager>();

        inventoryManager = FindObjectOfType<InventoryManager>();

        imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.grey;
        lineRenderer.endColor = Color.grey;
    }

    void Update()
    {
        if (currentObject == null) return;
        RaycastOnTrackedObject();
    }

    void RaycastOnTrackedObject()
    {
        Ray ray = GetRaycast();
        RaycastHit hit;

        Vector3 rayStart = ray.origin;
        Vector3 rayEnd = ray.origin + ray.direction * 2f;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (hitObj.CompareTag("Trap") || hitObj.CompareTag("Poision") || hitObj.CompareTag("Cat"))
            {
                if (Vector3.Distance(hit.point, currentObject.transform.position) < interactionDistance)
                    inventoryManager.PickupItem(hitObj);
            }

            if (hitObj.CompareTag("Rat"))
            {
                TryKillRat(hitObj, hit);
            }

            rayEnd = hit.point;
        }

        lineRenderer.SetPosition(0, rayStart);
        lineRenderer.SetPosition(1, rayEnd);
    }

    void TryKillRat(GameObject rat, RaycastHit hit)
    {
        if (inventoryManager == null) return;

        if (!inventoryManager.HasUsableItem())
        {
            Debug.Log("⚠️ Inget föremål i inventory – råttan dör inte.");
            return;
        }

        string usedItem = inventoryManager.GetFirstAvailableItem();

        if (usedItem != null && inventoryManager.UseItem(usedItem))
        {
            RatDestroyer destroyer = rat.GetComponentInParent<RatDestroyer>();
            if (destroyer != null)
            {
                destroyer.DieOnRaycastHit(hit, usedItem);
                SpawnBlood(rat.transform.position);
                PlaySound(ratDeathSound);
                ShowDeathMessage();
            }
            else
            {
                Debug.LogWarning("⚠️ Råttan saknar RatDestroyer-komponent.");
            }
        }
        else
        {
            Debug.Log("⚠️ Du har inget användbart föremål!");
        }
    }

    void SpawnBlood(Vector3 position)
    {
        if (bloodEffectPrefab != null)
        {
            GameObject splash = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
            ParticleSystem ps = splash.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startLifetime = 0.5f;
            }
            Destroy(splash, 1f);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void ShowDeathMessage()
    {
        if (deathMessageText != null)
        {
            deathMessageText.text = "YOU HAVE KILLED A RAT";
            StartCoroutine(HideDeathMessage());
        }
    }

    IEnumerator HideDeathMessage()
    {
        yield return new WaitForSeconds(2f);
        if (deathMessageText != null)
            deathMessageText.text = "";
    }

    private Ray GetRaycast()
    {
        if (currentObject == null)
        {
            Debug.LogWarning("Raycast försökte skapas men currentObject är null!");
            return new Ray(Vector3.zero, Vector3.forward);
        }

        return new Ray(currentObject.transform.position, currentObject.transform.forward);
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var image in args.added)
            CreateObjectOnImage(image);

        foreach (var image in args.updated)
        {
            if (currentObject != null)
            {
                currentObject.transform.position = image.transform.position;
                currentObject.transform.rotation = image.transform.rotation;
            }
        }
    }

    void CreateObjectOnImage(ARTrackedImage trackedImage)
    {
        if (currentObject == null)
        {
            currentObject = Instantiate(objectPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
        }
    }
}
