using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InteriorDataPoint
{
    // Variables in the config file
    public float intensity;
    public float drag;
    public Color color;

}

[System.Serializable]
public class InteriorData
{
    // Array of interior data points
    public InteriorDataPoint[] interiorData;
}

public class ExternalJSONConfig : MonoBehaviour
{
    // Path to the JSON config file
    public string configFilePath;

    // Current interior data point
    public InteriorDataPoint interiorDataPoint;

    // Signal when the JSON data is ready
    public delegate void JsonDataReadyCallback(InteriorDataPoint interiorData);
    public event JsonDataReadyCallback Ready;

    // How often (in seconds) to check for changes in the JSON file
    public float checkInterval = 5f;

    private string lastModifiedTimestamp;

    private void Start()
    {
        StartCoroutine(CheckForChanges());
    }

    private IEnumerator CheckForChanges()
    {
        while (true)
        {
            // Error handling
            if (string.IsNullOrEmpty(configFilePath))
            {
                Debug.Log("No config file path specified!");
                yield return new WaitForSeconds(checkInterval);
                continue;
            }

            if (!File.Exists(configFilePath))
            {
                Debug.Log("Config file does not exist at path: " + configFilePath);
                yield return new WaitForSeconds(checkInterval);
                continue;
            }

            // Get the last modified timestamp of the file
            string currentTimestamp = File.GetLastWriteTimeUtc(configFilePath).ToString();

            // Compare with the previous timestamp
            if (currentTimestamp != lastModifiedTimestamp)
            {
                lastModifiedTimestamp = currentTimestamp;

                // Read contents of JSON file specified at configFilePath
                string configString = File.ReadAllText(configFilePath);

                // Parse JSON string specified at configString, deserialize string & convert it to an instance of InteriorData
                InteriorData interiorDataWrapper = JsonUtility.FromJson<InteriorData>(configString);

                // interiorData is an array of InteriorDataPoint objects which store datapoints from JSON file
                InteriorDataPoint[] interiorData = interiorDataWrapper.interiorData;

                // Avoiding NullReferenceExceptions
                if (interiorData.Length > 0)
                {
                    interiorDataPoint = interiorData[0];
                    Ready?.Invoke(interiorDataPoint);   // Invoke the Ready event with the first interior data point
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}
