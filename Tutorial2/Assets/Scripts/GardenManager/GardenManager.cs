using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = gardenCamera.ScreenPointToRay(mouse);
        if (ground.Raycast(ray, out RaycastHit hit, 100f))
            Instantiate(plants[selectedPlant], hit.point, Quaternion.identity);
    }

    public void SelectPlant(int index)
    {
        selectedPlant = index;
    }
}
