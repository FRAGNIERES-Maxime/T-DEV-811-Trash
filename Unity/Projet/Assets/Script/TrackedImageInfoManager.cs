using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections;

/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some information as well as the source Texture2D on top of the
/// detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageInfoManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The camera to set on the world space UI canvas for each instantiated image info.")]
    Camera m_WorldSpaceCanvasCamera;
    public GameObject Garbage_vide;
    public GameObject Garbage_full;
    public GameObject TruckPrefab;

    private string TrackedImageName;
    private GameObject Garbage = null;
    private GameObject Truck = null;
    private bool EventGarbage = true;

    /// <summary>
    /// The prefab has a world space UI canvas,
    /// which requires a camera to function properly.
    /// </summary>
    public Camera worldSpaceCanvasCamera
    {
        get { return m_WorldSpaceCanvasCamera; }
        set { m_WorldSpaceCanvasCamera = value; }
    }

    [SerializeField]
    [Tooltip("If an image is detected but no source texture can be found, this texture is used instead.")]
    Texture2D m_DefaultTexture;

    /// <summary>
    /// If an image is detected but no source texture can be found,
    /// this texture is used instead.
    /// </summary>
    public Texture2D defaultTexture
    {
        get { return m_DefaultTexture; }
        set { m_DefaultTexture = value; }
    }

    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void UpdateInfo(ARTrackedImage trackedImage)
    {
        // Set canvas camera
        var canvas = trackedImage.GetComponentInChildren<Canvas>();
        canvas.worldCamera = worldSpaceCanvasCamera;

        // Update information about the tracked image
        var text = canvas.GetComponentInChildren<Text>();
        text.text = string.Format(
            "{0}\ntrackingState: {1}\nGUID: {2}\nReference size: {3} cm\nDetected size: {4} cm",
            trackedImage.referenceImage.name,
            trackedImage.trackingState,
            trackedImage.referenceImage.guid,
            trackedImage.referenceImage.size * 100f,
            trackedImage.size * 100f);

        var planeParentGo = trackedImage.transform.GetChild(0).gameObject;
        var planeGo = planeParentGo.transform.GetChild(0).gameObject;

        // Disable the visual plane if it is not being tracked
        
        if (trackedImage.trackingState != TrackingState.None)
        {
            planeGo.SetActive(true);

            // The image extents is only valid when the image is being tracked
            trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);

            // Set the texture
            var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
            material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;
        }
        else
        {
            planeGo.SetActive(false);
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            trackedImage.transform.localScale = new Vector3(1f, 1f, 1f);
            TrackedImageName = trackedImage.referenceImage.name;
            UpgradeGarbage(trackedImage);
            UpdateInfo(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            UpgradeGarbage(trackedImage);
            UpdateInfo(trackedImage);
        }
    }

    void UpgradeGarbage(ARTrackedImage trackedImage)
    {
        if (TrackedImageName == "Matrice_ON")
        {
            if (!Garbage)
            {
                Garbage = Instantiate(Garbage_full, trackedImage.transform.localPosition, Garbage_full.transform.localRotation);
                Garbage.transform.localScale = new Vector3(5f, 5f, 5f);
                Garbage.transform.localScale = trackedImage.transform.localScale;
                Garbage.transform.parent = trackedImage.transform;
                Garbage.transform.localPosition = Vector3.zero;
            }
            else
                Garbage = Garbage_full;
            if (!Truck)
            {
                Truck = Instantiate(TruckPrefab, trackedImage.transform.localPosition, TruckPrefab.transform.localRotation);
                Truck.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                Truck.transform.parent = trackedImage.transform;
                Truck.transform.localPosition = Vector3.zero;
            }
        }
        else if (TrackedImageName == "Matrice_OFF")
        {
            if (!Garbage)
            {
                Garbage = Instantiate(Garbage_vide, trackedImage.transform.localPosition, Garbage_vide.transform.localRotation);
                Garbage.transform.localScale = new Vector3(5f, 5f, 5f);
                Garbage.transform.localScale = trackedImage.transform.localScale;
                Garbage.transform.parent = trackedImage.transform;
                Garbage.transform.localPosition = Vector3.zero;
            }
            else
                Garbage = Garbage_vide;
        }
        EventGarbage = false;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), TrackedImageName);
    }

    IEnumerator Clean_Garbage()
    {
        yield return new WaitForSeconds(10f);
        Destroy(Truck);
        Garbage = Garbage_vide;
    }
}
