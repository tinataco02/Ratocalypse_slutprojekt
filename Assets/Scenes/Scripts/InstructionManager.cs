using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class InstructionTextWithGesture : MonoBehaviour
{
    public TextMeshProUGUI instructionText; 
    public ARTrackedImageManager trackedImageManager;
    public float movementThreshold = 0.01f; 

    private Dictionary<string, Vector3> previousPositions = new Dictionary<string, Vector3>();
    private bool gameStarted = false;

    void Start()
    {
        Time.timeScale = 0; 
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking && !gameStarted)
            {
                DetectMotion(trackedImage);
            }
            
        }
    }

    private void DetectMotion(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;
        Vector3 currentPosition = trackedImage.transform.position;

        if (previousPositions.ContainsKey(imageName))
        {
            Vector3 movement = currentPosition - previousPositions[imageName];

            if (movement.y > movementThreshold)
            {
                StartGame();
            }
        }

        previousPositions[imageName] = currentPosition;
    }

    private void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1; 
        HideInstructionText();
    }

    private void HideInstructionText()
    {
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }
}
