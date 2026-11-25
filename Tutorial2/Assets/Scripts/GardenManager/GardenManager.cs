using UnityEngine;
using UnityEngine.InputSystem;

public class GardenManager : MonoBehaviour
{
    [Header("Planting")]
    public GameObject[] plants;
    public Camera gardenCamera;
    public Collider ground;

    private int selectedPlant;

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mouse = Mouse.current.position.ReadValue();
        if (mouse.y > Screen.height - 80) return;

        Ray ray = gardenCamera.ScreenPointToRay(mouse);
        if (ground.Raycast(ray, out RaycastHit hit, 100f))
            Instantiate(plants[selectedPlant], hit.point, Quaternion.identity);
    }

    private void OnGUI()
    {
        for (int i = 0; i < plants.Length; i++)
            if (GUI.Button(new Rect(10 + i * 130, 10, 120, 40), plants[i].name))
                selectedPlant = i;

        GUI.Label(new Rect(10, 55, 440, 25), "Selected: " + plants[selectedPlant].name + " — click ground to plant");
    }
}
