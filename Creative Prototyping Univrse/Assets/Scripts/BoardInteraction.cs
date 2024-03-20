using System;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class BoardInteraction : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRendererLeft;
    [SerializeField] private LineRenderer lineRendererRight;
    [SerializeField] private Transform controllerTransformLeft;
    [SerializeField] private Transform controllerTransformRight;
    [SerializeField] private LayerMask boardLayer; // Layer mask for the board
    [SerializeField] private float interactionDistance = 2.5f;

    [SerializeField] private Button windowsSynthButton;
    [SerializeField] private Button elevenLabsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Dropdown microDropdown;

    //For button down
    private bool previousLeftTriggerState = false;
    private bool previousRightTriggerState = false;

    private void Start()
    {
        //We init the button functions
        windowsSynthButton.onClick.AddListener(() => ElevenLabsOrWindowsSynth(false));
        elevenLabsButton.onClick.AddListener(() => ElevenLabsOrWindowsSynth(true));
        exitButton.onClick.AddListener(ExitGame);

        //We init the dropdown
        PopulateDropdown();
        microDropdown.value = SearchDropdownIndexByName(PlayerSpeechRecord.microphoneOption);
    }

    public void ElevenLabsOrWindowsSynth(bool status) { Settings.Instance.useElevenLabs = status; }
    public void ExitGame() { Application.Quit(); }
    private void PopulateDropdown()
    {
        List<string> options = new List<string>();

        // Get the list of available microphones
        string[] devices = Microphone.devices;

        // Add each microphone to the dropdown options
        foreach (string device in devices)
        {
            options.Add(device);
        }

        // Clear existing options and add new options to the dropdown
        microDropdown.ClearOptions();
        microDropdown.AddOptions(options);
    }
    private void HandleDropdownValueChanged(int index)
    {
        string selectedOption = microDropdown.options[index].text;
        PlayerSpeechRecord.UpdateMicrophone(selectedOption);
        microDropdown.value = index;
        microDropdown.Hide();
    }
    private int SearchDropdownIndexByName(string name)
    {
        for(int i = 0; i < Microphone.devices.Length; i++)
        {
            if (Microphone.devices[i].Equals(name))
                return i;
        }

        return 0;   //Get the first microphone in case it does not find any
    }

    private void Update()
    {
        HandleRaycast(controllerTransformLeft, lineRendererLeft);
        HandleRaycast(controllerTransformRight, lineRendererRight);

        bool leftTrigger;
        if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTrigger))
        {
            if (leftTrigger && !previousLeftTriggerState)
            {
                TryInteractWithButton(controllerTransformLeft);
            }
        }

        bool rightTrigger;
        if (UnityEngine.XR.InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTrigger))
        {
            if (rightTrigger && !previousRightTriggerState)
            {
                TryInteractWithButton(controllerTransformRight);
            }
        }

        previousLeftTriggerState = leftTrigger;
        previousRightTriggerState = rightTrigger;
    }

    private void HandleRaycast(Transform controllerTransform, LineRenderer lineRenderer)
    {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, interactionDistance, boardLayer))
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, controllerTransform.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void TryInteractWithButton(Transform controllerTransform)
    {
        RaycastHit hit;
        if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, out hit, interactionDistance))
        {
            Toggle optionDropdown = hit.collider.GetComponent<Toggle>();
            if (optionDropdown != null)
            {
                HandleDropdownValueChanged(SearchDropdownIndexByName(optionDropdown.transform.Find("Item Label").GetComponent<TMP_Text>().text));
            }
            else
            {
                // Check if the object hit is a dropdown
                TMP_Dropdown dropdown = hit.collider.GetComponent<TMP_Dropdown>();
                if (dropdown != null)
                {
                    // Open the dropdown
                    dropdown.Show();

                    //Adjust the dropdown visually
                    RectTransform drowpownTransform = dropdown.transform.Find("Dropdown List").GetComponent<RectTransform>();
                    drowpownTransform.anchorMin = new Vector2(0,0);
                    drowpownTransform.anchorMax = new Vector2(1, 0);
                    drowpownTransform.pivot = new Vector2(0.5f, 1);
                    drowpownTransform.anchoredPosition = new Vector2(0f, 2);

                    GameObject parent = dropdown.transform.Find("Dropdown List").transform.GetChild(0).transform.GetChild(0).gameObject;
                    // Add BoxCollider to each dropdown option
                    for(int i = 0; i<Microphone.devices.Length; i++)
                    {
                        string idx = $"Item {i}: {Microphone.devices[i]}";
                        GameObject itemGameObject = parent.transform.Find(idx).gameObject;
                        if (itemGameObject.GetComponent<BoxCollider>() == null)
                        {
                            BoxCollider boxCollider = itemGameObject.AddComponent<BoxCollider>();
                            // Adjust collider size and position as per your requirement
                            boxCollider.size = new Vector3(135f, 20f, 1f); // Adjust size as needed
                            boxCollider.center = new Vector3(0f, 0f, -0.03f); // Adjust center as needed
                        }
                        if (itemGameObject.GetComponent<DropdownReference>() == null)
                        {
                            DropdownReference dropdownReference = itemGameObject.AddComponent<DropdownReference>();
                            dropdownReference.parent = dropdown;
                        }
                    }
                }
                else
                {
                    // Comprobar si el objeto golpeado es un botón
                    Button button = hit.collider.GetComponent<Button>();
                    if (button != null)
                    {
                        // Llamar al método para interactuar con el botón
                        button.onClick.Invoke();
                    }
                }
            }
        }
    }
}
