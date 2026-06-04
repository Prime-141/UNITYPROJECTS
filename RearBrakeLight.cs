using UnityEngine;

public class RearBrakeLight : MonoBehaviour
{
    private Light[] rearLights;

    void Start()
    {
        // Get both L & R lights automatically
        rearLights = GetComponentsInChildren<Light>();

        SetLights(false); // OFF at start
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.S))
            SetLights(true);
        else
            SetLights(false);
    }

    void SetLights(bool state)
    {
        foreach (Light light in rearLights)
        {
            if (light != null)
                light.enabled = state;
        }
    }
}
