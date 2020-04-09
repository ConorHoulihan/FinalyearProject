using UnityEngine;

public class Exposition : MonoBehaviour
{

    public Canvas canvas;
    void Update()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (go.GetComponentInChildren<UnityEngine.Camera>().enabled)
                canvas.worldCamera = go.GetComponentInChildren<UnityEngine.Camera>();
        }
    }
}
