using UnityEngine;

public class SkyboxHandler : MonoBehaviour
{
    public Material[] Skyboxes;

    public float RotationPerSecond = 1;
    private bool rotate = true;

    public void Start()
    {
        ChangeSkybox();
    }

    protected void Update()
    {
        if (rotate) {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * RotationPerSecond);
        }
    }

    public void ToggleRotation()
    {
        rotate = !rotate;
    }

    public void ChangeSkybox()
    {
        RenderSettings.skybox = Skyboxes[(int) (Random.value * (Skyboxes.Length - 1))];
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }
}