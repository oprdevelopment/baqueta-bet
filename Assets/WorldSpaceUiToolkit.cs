using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.UIElements
{
[RequireComponent(typeof(UIDocument))]
public class WorldSpaceUIToolkit : MonoBehaviour
{
    [SerializeField] private Camera inputCamera;
    [SerializeField] private Collider uiCollider;

    private UIDocument document;
    private PanelSettings panelSettings;
    Vector2 lastPos = new(float.NaN, float.NaN);

    void Awake()
    {
        document = GetComponent<UIDocument>();
        panelSettings = document.panelSettings;

        panelSettings.SetScreenToPanelSpaceFunction(ScreenToPanel);
    }

    Vector2 ScreenToPanel(Vector2 screenPosition)
    {
        if (inputCamera == null || uiCollider == null)
        {
            Debug.Log("asd");
            return lastPos;
        }

        Ray ray = inputCamera.ScreenPointToRay(screenPosition);
        if (!uiCollider.Raycast(ray, out RaycastHit hit, 1000f))
        {
            return lastPos;
        }

        Vector2 uv = hit.textureCoord;
        Vector2 panelPos = new(
            uv.x * panelSettings.targetTexture.width,
            uv.y * panelSettings.targetTexture.height
        );

        lastPos = panelPos;
        return lastPos;
    }
}
}