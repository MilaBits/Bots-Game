using TMPro;
using UnityEngine;

public class DebugWorldSpaceUI : MonoBehaviour
{
    public Vector3 offset;
    private Canvas _canvas;
    private TextMeshProUGUI _textMeshProUGUI;
    private string text;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            _textMeshProUGUI.text = text;
        }
    }

    private void Update()
    {
        _canvas.transform.LookAt(Camera.main.transform);
    }
}