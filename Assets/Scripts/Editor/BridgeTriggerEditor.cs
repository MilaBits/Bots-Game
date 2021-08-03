using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BridgeTrigger))]
public class BridgeTriggerEditor : Editor
{
    private BridgeTrigger _bridgeTrigger;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _bridgeTrigger = target as BridgeTrigger;

        if (GUILayout.Button("Update"))
        {
            SetWidth();
        }
    }

    private void SetWidth()
    {
        _bridgeTrigger.BoxCollider.size = new Vector3(1, 1, _bridgeTrigger.Width);

        foreach (Transform child in _bridgeTrigger.MeshContainer.transform.Cast<Transform>().ToList())
        {
            DestroyImmediate(child.gameObject);
        }

        float StartOffset = -(_bridgeTrigger.Width / 2);
        if (_bridgeTrigger.Width % 2 == 0) StartOffset += .5f;
        GameObject current;
        for (int i = 0; i < _bridgeTrigger.Width; i++)
        {
            if (i == 0)
            {
                current = Instantiate(_bridgeTrigger.Cap, _bridgeTrigger.MeshContainer);
                current.transform.localPosition = new Vector3(0, 0, StartOffset + i);
                current.transform.rotation = Quaternion.Euler(0, 180, 0);
                continue;
            }

            if (i == _bridgeTrigger.Width - 1)
            {
                current = Instantiate(_bridgeTrigger.Cap, _bridgeTrigger.MeshContainer);
                current.transform.localPosition = new Vector3(0, 0, StartOffset + i);
                current.transform.rotation = Quaternion.identity;
                continue;
            }

            current = Instantiate(_bridgeTrigger.Segment, new Vector3(0, 0, StartOffset + i), Quaternion.identity, _bridgeTrigger.MeshContainer);
            current.transform.localPosition = new Vector3(0, 0, StartOffset + i);
        }
    }
}