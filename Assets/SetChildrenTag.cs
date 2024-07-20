using UnityEngine;

public class SetChildrenLayer : MonoBehaviour
{
    // The layer you want to set for all children
    public int layerToSet = 26;

    void Update()
    {
        SetLayerForAllChildren(transform, layerToSet);
    }

    void SetLayerForAllChildren(Transform parent, int newLayer)
    {
        // Iterate through all children of the parent
        foreach (Transform child in parent)
        {
            // Set the layer for the child
            child.gameObject.layer = newLayer;

            // Recursively set the layer for any children of this child
            SetLayerForAllChildren(child, newLayer);
        }
    }
}
