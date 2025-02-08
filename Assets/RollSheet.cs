using UnityEngine;

public class SpiralUnroll : MonoBehaviour {
    public float unrollAmount = 0f; // Valore tra 0 (arrotolato) e 1 (srotolato)
    public float spiralTightness = 1.5f; // Quanto stretta è la spirale
    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = mesh.vertices.Clone() as Vector3[];
    }

    void Update() {
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            float t = originalVertices[i].y * unrollAmount;
            float angle = t * Mathf.PI * spiralTightness; // Angolo della spirale

            vertices[i].x = originalVertices[i].x * Mathf.Cos(angle);
            vertices[i].z = originalVertices[i].x * Mathf.Sin(angle);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}
