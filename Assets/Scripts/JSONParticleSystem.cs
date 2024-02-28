using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX; // Add this line for VFX Graph

public class JSONVFXGraph : MonoBehaviour
{
    // Reference to the ExternalJSONConfig script
    public ExternalJSONConfig jsonConfig;

    // Reference to the VisualEffect component
    public VisualEffect visualEffectJSON; // Use VisualEffect instead of ParticleSystem

    private void Start()
    {
        // Error handling
        if (jsonConfig == null)
        {
            Debug.Log("No ExternalJSONConfig assigned!");
            return;
        }

        if (visualEffectJSON == null)
        {
            Debug.Log("No VisualEffect assigned!");
            return;
        }

        // Subscribe to the Ready event of ExternalJSONConfig
        jsonConfig.Ready += UpdateVisualEffectColor;
    }

    private void OnDestroy()
    {
        if (jsonConfig != null)
        {
            // Unsubscribe from the Ready event of ExternalJSONConfig
            jsonConfig.Ready -= UpdateVisualEffectColor;
        }
    }

    private void UpdateVisualEffectColor(InteriorDataPoint interiorDataPoint)
    {
        Color color = interiorDataPoint.color;
        float intensity = interiorDataPoint.intensity;
        float drag = interiorDataPoint.drag;
        Debug.Log(color);
        Debug.Log(intensity);
        Debug.Log(drag);

        visualEffectJSON.SetVector4("Color", new Vector4(color.r, color.g, color.b, color.a));
        visualEffectJSON.SetFloat("Intensity", intensity);
        visualEffectJSON.SetFloat("Drag", drag);
    }
}
