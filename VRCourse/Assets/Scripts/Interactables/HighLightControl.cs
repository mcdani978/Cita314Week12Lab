using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HighlightControl : MonoBehaviour
{
    [SerializeField] XRBaseInteractable interactableObject;
    [SerializeField] Material startMaterial;
    [SerializeField] Material emmisionMaterial;
    [SerializeField] Renderer highlightableObject;

    private void OnEnable()
    {
        if (interactableObject != null)
        {
            interactableObject.selectEntered.AddListener(HighlightObject);
            interactableObject.selectExited.AddListener(ResetObject);
        }
    }

    private void ResetObject(SelectExitEventArgs args)
    {
        if (highlightableObject != null && startMaterial != null)
        {
            highlightableObject.material = startMaterial;
        }
    }

    private void OnDisable()
    {
        if (interactableObject != null)
        {
            interactableObject.selectEntered.RemoveListener(HighlightObject);
            interactableObject.selectExited.RemoveListener(ResetObject);
        }
    }

    private void HighlightObject(SelectEnterEventArgs args)
    {
        if (highlightableObject != null && emmisionMaterial != null)
        {
            highlightableObject.material = emmisionMaterial;
        }
    }
}
