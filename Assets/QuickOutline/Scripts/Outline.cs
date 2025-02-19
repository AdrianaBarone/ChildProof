using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [Header("Outline Settings")]
    [SerializeField] private Mode outlineMode;
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField, Range(0f, 10f)] private float outlineWidth = 2f;

    [Header("Optional")]
    [Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
           + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    public bool precomputeOutline;

    [SerializeField, HideInInspector] private List<Mesh> bakeKeys = new List<Mesh>();
    [SerializeField, HideInInspector] private List<ListVector3> bakeValues = new List<ListVector3>();

    // ===========================================
    // Per escludere solo CERTI oggetti (figli):
    [Tooltip("Seleziona l'oggetto da escludere dall'outline. "
           + "Se un Renderer appartiene a questo oggetto o ne è un discendente, NON avrà l'outline.")]
    public GameObject objectToExclude;
    // Se vuoi anche usare un tag, ad esempio "NoOutline", decommenta o aggiungi logica qui.
    // ===========================================

    private Renderer[] allRenderers;
    private Material outlineMaskMaterial;
    private Material outlineFillMaterial;

    private bool needsUpdate;

    public Mode OutlineMode
    {
        get => outlineMode;
        set { outlineMode = value; needsUpdate = true; }
    }

    public Color OutlineColor
    {
        get => outlineColor;
        set { outlineColor = value; needsUpdate = true; }
    }

    public float OutlineWidth
    {
        get => outlineWidth;
        set { outlineWidth = value; needsUpdate = true; }
    }

    // Metodo per dire se un singolo Renderer (figlio) va escluso
    private bool ShouldExcludeRenderer(Renderer r)
    {
        if (objectToExclude != null)
        {
            // Se il renderer appartiene a 'objectToExclude' (o ne è un discendente), lo saltiamo
            if (r.gameObject == objectToExclude || r.transform.IsChildOf(objectToExclude.transform))
            {
                return true;
            }
        }

        // Se preferisci anche un controllo sul tag "NoOutline", aggiungi:
        // if (HasNoOutlineTag(r.gameObject)) return true;

        return false;
    }

    // Se vuoi gestire i tag (opzionale):
    // private bool HasNoOutlineTag(GameObject obj)
    // {
    //     // Qui potresti controllare anche i genitori, se vuoi
    //     // Transform t = obj.transform;
    //     // while (t != null)
    //     // {
    //     //     if (t.gameObject.CompareTag("NoOutline"))
    //     //         return true;
    //     //     t = t.parent;
    //     // }
    //     // return false;
    //
    //     return obj.CompareTag("NoOutline");
    // }

    void Awake()
    {
        // Trova TUTTI i renderer figli
        allRenderers = GetComponentsInChildren<Renderer>(true);

        // Istanzia i materiali per l'outline
        outlineMaskMaterial = Instantiate(Resources.Load<Material>("Materials/OutlineMask"));
        outlineFillMaterial = Instantiate(Resources.Load<Material>("Materials/OutlineFill"));

        outlineMaskMaterial.name = "OutlineMask (Instance)";
        outlineFillMaterial.name = "OutlineFill (Instance)";

        // Carica o genera le smooth normals
        LoadSmoothNormals();

        // Forziamo l'aggiornamento delle proprietà
        needsUpdate = true;
    }

    void OnEnable()
    {
        // Aggiunge gli shader solo ai Renderer che NON vanno esclusi
        foreach (var r in allRenderers)
        {
            if (ShouldExcludeRenderer(r)) 
                continue; // salta questo renderer

            var materials = r.sharedMaterials.ToList();
            if (!materials.Contains(outlineMaskMaterial))
            {
                materials.Add(outlineMaskMaterial);
            }
            if (!materials.Contains(outlineFillMaterial))
            {
                materials.Add(outlineFillMaterial);
            }
            r.materials = materials.ToArray();
        }
    }

    void OnValidate()
    {
        needsUpdate = true;

        // Pulisci la cache se il bake è disabilitato o corrotto
        if (!precomputeOutline && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
        {
            bakeKeys.Clear();
            bakeValues.Clear();
        }

        // Genera smooth normals se il bake è abilitato e non ci sono dati
        if (precomputeOutline && bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    void Update()
    {
        if (needsUpdate)
        {
            needsUpdate = false;
            UpdateMaterialProperties();
        }
    }

    void OnDisable()
    {
        // Rimuove gli shader solo dai Renderer che NON vanno esclusi
        foreach (var r in allRenderers)
        {
            if (ShouldExcludeRenderer(r)) 
                continue; // salta questo renderer

            var materials = r.sharedMaterials.ToList();
            if (materials.Contains(outlineMaskMaterial))
            {
                materials.Remove(outlineMaskMaterial);
            }
            if (materials.Contains(outlineFillMaterial))
            {
                materials.Remove(outlineFillMaterial);
            }
            r.materials = materials.ToArray();
        }
    }

    void OnDestroy()
    {
        // Distrugge le istanze dei materiali
        Destroy(outlineMaskMaterial);
        Destroy(outlineFillMaterial);
    }

    void Bake()
    {
        // Genera smooth normals per ogni MeshFilter figlio
        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>(true))
        {
            // Se vogliamo evitare di generare UV3 per i renderer esclusi, controlliamo:
            var rend = meshFilter.GetComponent<Renderer>();
            if (rend != null && ShouldExcludeRenderer(rend)) 
                continue;

            // Salta duplicati
            if (!bakedMeshes.Add(meshFilter.sharedMesh)) 
                continue;

            // Calcola e serializza le smooth normals
            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);
            bakeKeys.Add(meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3 { data = smoothNormals });
        }
    }

    void LoadSmoothNormals()
    {
        // Recupera o genera le smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>(true))
        {
            // Se vogliamo evitare di generare UV3 per i renderer esclusi, controlliamo:
            var rend = meshFilter.GetComponent<Renderer>();
            if (rend != null && ShouldExcludeRenderer(rend)) 
                continue;

            if (!registeredMeshes.Add(meshFilter.sharedMesh)) 
                continue;

            var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) 
                                ? bakeValues[index].data 
                                : SmoothNormals(meshFilter.sharedMesh);

            // Salva le smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // Combina i submesh
            if (rend != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, rend.sharedMaterials);
            }
        }

        // Stessa logica per SkinnedMeshRenderer
        foreach (var skinnedRenderer in GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (ShouldExcludeRenderer(skinnedRenderer))
                continue;

            if (!registeredMeshes.Add(skinnedRenderer.sharedMesh)) 
                continue;

            skinnedRenderer.sharedMesh.uv4 = new Vector2[skinnedRenderer.sharedMesh.vertexCount];
            CombineSubmeshes(skinnedRenderer.sharedMesh, skinnedRenderer.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        // Raggruppa i vertici per posizione
        var groups = mesh.vertices
                         .Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index))
                         .GroupBy(pair => pair.Key);

        // Copia le normali
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Calcola la normale media per i gruppi di vertici
        foreach (var group in groups)
        {
            if (group.Count() == 1) 
                continue;

            var smoothNormal = Vector3.zero;
            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }
            smoothNormal.Normalize();

            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }
        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1) 
            return;

        if (mesh.subMeshCount > materials.Length) 
            return;

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    void UpdateMaterialProperties()
    {
        // Colore
        outlineFillMaterial.SetColor("_OutlineColor", outlineColor);

        // ZTest e larghezza a seconda della modalità
        switch (outlineMode)
        {
            case Mode.OutlineAll:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineVisible:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineHidden:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                outlineFillMaterial.SetFloat("_OutlineWidth", 0f);
                break;
        }
    }
}
