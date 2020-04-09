using UnityEngine;


public class IgnoreUiRaycastWhenInactive : MonoBehaviour, ICanvasRaycastFilter
{
    public bool IsRaycastLocationValid(Vector2 screenPoint, UnityEngine.Camera eventCamera)
    {
        return gameObject.activeInHierarchy;
    }
}