using System;
using System.Collections.Generic;
using TMPro;
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

        exitButton.onClick.Invoke();

        //We init the dropdown
        PopulateDropdown();
        microDropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
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
        Debug.Log("Selected option: " + selectedOption);
        PlayerSpeechRecord.microphoneOption = selectedOption;
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
            // Check if the object hit is a dropdown
            TMP_Dropdown dropdown = hit.collider.GetComponent<TMP_Dropdown>();
            if (dropdown != null)
            {
                // Open the dropdown
                dropdown.Show();

                // Check if the hit point is inside the dropdown's rect transform
                RectTransform dropdownRect = dropdown.gameObject.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(dropdownRect, hit.point))
                {
                    // Calculate local position within the dropdown
                    Vector2 localPosition;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(dropdownRect, hit.point, null, out localPosition);

                    // Calculate which option was hit
                    int optionIndex = GetDropdownOptionIndex(dropdown, localPosition);

                    // Set the dropdown's value to the selected option index
                    dropdown.value = optionIndex;

                    // Trigger the dropdown's OnValueChanged event
                    dropdown.onValueChanged.Invoke(optionIndex);
                }
            }
            else
            {
                // Check if the object hit is a button
                Button button = hit.collider.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                }
            }
        }
    }

    private int GetDropdownOptionIndex(TMP_Dropdown dropdown, Vector2 localPosition)
    {
        RectTransform dropdownRect = dropdown.GetComponent<RectTransform>();
        if (dropdownRect != null)
        {
            // Calculate the local position of the hit point within the dropdown's rect transform
            Vector2 normalizedPosition = new Vector2(
                Mathf.InverseLerp(dropdownRect.rect.xMin, dropdownRect.rect.xMax, localPosition.x),
                Mathf.InverseLerp(dropdownRect.rect.yMin, dropdownRect.rect.yMax, localPosition.y));

            // Calculate the option index based on the normalized position
            int optionIndex = Mathf.FloorToInt(normalizedPosition.y * dropdown.options.Count);

            // Clamp the option index within the range of valid indices
            optionIndex = Mathf.Clamp(optionIndex, 0, dropdown.options.Count - 1);

            return optionIndex;
        }

        return -1; // No option was hit or dropdown rect transform is null
    }

}
