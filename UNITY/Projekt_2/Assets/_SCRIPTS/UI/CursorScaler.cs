using System;
using UnityEngine.UI;
using UnityEngine;

public class CursorScaler : MonoBehaviour
{
    [SerializeField] Texture2D _cursorSprite;

    Vector2 _cursorHotspot = new Vector2(0.5f, 0.5f);

    void Awake()
    {
        Cursor.SetCursor(_cursorSprite, _cursorHotspot, CursorMode.ForceSoftware);
    }
}
