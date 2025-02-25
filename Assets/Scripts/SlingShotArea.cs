using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{
    [SerializeField] private LayerMask slingshotAreaMask;
    public bool IsWithinSlingshotArea()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Physics2D.OverlapPoint(worldPosition,slingshotAreaMask))
        {
            return true;
        }

        else
        {
            return false;
        }
    }

}
