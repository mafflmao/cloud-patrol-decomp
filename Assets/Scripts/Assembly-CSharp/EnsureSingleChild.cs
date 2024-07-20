using UnityEngine;

public class EnsureSingleChild : MonoBehaviour
{
    void Update()
    {
        EnsureOnlyOneChild();
    }

    private void EnsureOnlyOneChild()
    {
        // Check if the object has more than one child
        if (transform.childCount > 1)
        {
            // Loop through all children except the first one
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                // Destroy the extra children
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
