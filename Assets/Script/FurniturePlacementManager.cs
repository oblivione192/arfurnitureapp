using Unity.XR.CoreUtils;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FurniturePlacementManager : MonoBehaviour
{
    public GameObject spawnableFurniture;
    public GameObject spawned;
    public XROrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject deleteBtn;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private float initialTouchAngle;
    private Quaternion initialRotation;

    private void Start()
    {
        deleteBtn.SetActive(false); // Hide button at app start
    }

    private void Update()
    {
        // Handle placement
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !isButtonPressed())
            {
                if (raycastManager.Raycast(touch.position, raycastHits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = raycastHits[0].pose;

                    if (spawned == null)
                    {
                        spawned = Instantiate(spawnableFurniture, hitPose.position, hitPose.rotation);
                        deleteBtn.SetActive(true);
                    }
                    else
                    {
                        spawned.transform.position = hitPose.position;
                        spawned.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }

        // Handle rotation
        if (spawned != null && Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Only handle rotation on movement
            if (touch1.phase == TouchPhase.Began)
            {
                initialTouchAngle = Vector2.SignedAngle(touch0.position - touch1.position, Vector2.right);
                initialRotation = spawned.transform.rotation;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float currentTouchAngle = Vector2.SignedAngle(touch0.position - touch1.position, Vector2.right);
                float angleDelta = currentTouchAngle - initialTouchAngle;

                spawned.transform.rotation = initialRotation * Quaternion.Euler(0f, -angleDelta, 0f);
            }
        }
    }

    public bool isButtonPressed()
    {
        return EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() != null;
    }

    public void SwitchFurniture(GameObject furniture)
    {
        spawnableFurniture = furniture;
    }

    public void DeleteSpawned()
    {
        if (spawned != null)
        {
            Destroy(spawned);
            spawned = null;
            deleteBtn.SetActive(false);
        }
    }
}
