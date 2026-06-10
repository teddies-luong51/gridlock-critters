using UnityEngine;

/// <summary>Editor-only keyboard fallback: arrow keys drive the board so tilt can be tested without a device.</summary>
public class EditorTiltInput : MonoBehaviour
{
#if UNITY_EDITOR
    private void Update()
    {
        if (BoardController.Instance == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))  BoardController.Instance.ExecuteTilt(TiltDirection.Left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) BoardController.Instance.ExecuteTilt(TiltDirection.Right);
        if (Input.GetKeyDown(KeyCode.UpArrow))    BoardController.Instance.ExecuteTilt(TiltDirection.Up);
        if (Input.GetKeyDown(KeyCode.DownArrow))  BoardController.Instance.ExecuteTilt(TiltDirection.Down);
    }
#endif
}
