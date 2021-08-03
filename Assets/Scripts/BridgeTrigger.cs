using UnityEngine;

[ExecuteInEditMode]
public class BridgeTrigger : MonoBehaviour
{
    [Range(2, 16)] public int Width = 4;

    [SerializeField] public GameObject Cap;
    [SerializeField] public GameObject Segment;

    [SerializeField] public BoxCollider BoxCollider;
    [SerializeField] public Transform MeshContainer;
}