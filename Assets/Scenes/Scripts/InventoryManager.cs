using UnityEngine;
using TMPro;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
    private int trapCount = 0;
    private int poisionCount = 0;
    private int catCount = 0;

    public TextMeshProUGUI inventoryText;
    public TextMeshProUGUI trapCountText;
    public TextMeshProUGUI poisionCountText;
    public TextMeshProUGUI catCountText;
    public AudioSource audioSource;
    public AudioClip catPickupSound;
    public AudioClip trapPickupSound;
    public AudioClip poisonPickupSound;
    public TextMeshProUGUI warningText;

    public void PickupItem(GameObject item)
    {
        if (item.CompareTag("Trap"))
        {
            trapCount++;
            PlaySound(trapPickupSound);
        }
        else if (item.CompareTag("Poision"))
        {
            poisionCount++;
            PlaySound(poisonPickupSound);
        }
        else if (item.CompareTag("Cat"))
        {
            catCount++;
            PlaySound(catPickupSound);
        }

        Destroy(item);
        UpdateUI();
    }

    public bool HasUsableItem()
    {
        return trapCount > 0 || poisionCount > 0 || catCount > 0;
    }

    public string GetFirstAvailableItem()
    {
        if (poisionCount > 0) return "Poision";
        if (trapCount > 0) return "Trap";
        if (catCount > 0) return "Cat";
        return null;
    }

    public bool UseItem(string itemType)
    {
        if (itemType == "Trap" && trapCount > 0)
        {
            trapCount--;
            UpdateUI();
            return true;
        }
        else if (itemType == "Poision" && poisionCount > 0)
        {
            poisionCount--;
            UpdateUI();
            return true;
        }
        else if (itemType == "Cat" && catCount > 0)
        {
            catCount--;
            UpdateUI();
            return true;
        }

        return false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void UpdateUI()
    {
        if (trapCountText != null)
            trapCountText.text = $"{trapCount}";

        if (poisionCountText != null)
            poisionCountText.text = $"{poisionCount}";

        if (catCountText != null)
            catCountText.text = $"{catCount}";
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.color = Color.red;
            StartCoroutine(HideWarning());
        }
    }

    private IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(2f);
        if (warningText != null)
        {
            warningText.text = "";
        }
    }
}
