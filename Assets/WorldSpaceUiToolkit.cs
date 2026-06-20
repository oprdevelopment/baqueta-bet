using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class WorldSpaceUIToolkit : MonoBehaviour
{
    [SerializeField] private Camera inputCamera;
    [SerializeField] private Collider uiCollider;

    private UIDocument document;
    private PanelSettings panelSettings;

    void Awake()
    {
        document = GetComponent<UIDocument>();
        panelSettings = document.panelSettings;

        panelSettings.SetScreenToPanelSpaceFunction(ScreenToPanel);
    }

    Vector2 ScreenToPanel(Vector2 screenPosition)
    {
        if (inputCamera == null || uiCollider == null)
            return new Vector2(float.NaN, float.NaN);

        Ray ray = inputCamera.ScreenPointToRay(screenPosition);
        if (!uiCollider.Raycast(ray, out RaycastHit hit, 1000f))
                return new Vector2(float.NaN, float.NaN);

        Vector2 uv = hit.textureCoord;
        Debug.Log(uv);
        Vector2 panelPos = new(
            uv.x * panelSettings.targetTexture.width,
            uv.y * panelSettings.targetTexture.height
        );
        
        return panelPos;
    }
}