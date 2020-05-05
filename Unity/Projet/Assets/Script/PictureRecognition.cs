using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class PictureRecognition : MonoBehaviour
{
    public GameObject Garbage_vide;
    public GameObject Garbage_full;

    private ARTrackedImageManager _arTrackedImageManager;
    private GameObject Garbage;

    private void awake()
    {
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    public void OnEnable()
    {
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            Debug.Log(trackedImage.name);
            if (trackedImage.name == "Matrice_ON")
            {
                //Destroy(Garbage);
                Garbage = Instantiate(Garbage_full);
            }
            else if (trackedImage.name == "Matrice_OFF")
            {
                //Destroy(Garbage);
                Garbage = Instantiate(Garbage_vide);
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
