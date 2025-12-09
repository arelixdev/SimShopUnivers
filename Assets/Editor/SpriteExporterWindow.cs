using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteExporterWindow : EditorWindow
{
    private List<GameObject> prefabsToExport = new List<GameObject>();

    private Camera captureCamera;

    private int resolution = 1024;
    private float targetSize = 1f;
    private string outputFolder = "SpritesExported";

    private Vector3 cameraRotation = new Vector3(0, 0, 0);

    [MenuItem("Tools/Sprite Exporter")]
    public static void ShowWindow()
    {
        GetWindow<SpriteExporterWindow>("Sprite Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("EXPORTER PLUSIEURS PREFABS EN PNG", EditorStyles.boldLabel);

        GUILayout.Space(10);

        // --- Liste des prefabs ---
        GUILayout.Label("Liste des Prefabs à exporter :", EditorStyles.boldLabel);

        if (prefabsToExport.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucun prefab ajouté.", MessageType.Info);
        }

        // Affiche la liste
        for (int i = 0; i < prefabsToExport.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            prefabsToExport[i] = (GameObject)EditorGUILayout.ObjectField(prefabsToExport[i], typeof(GameObject), false);

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                prefabsToExport.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        // Bouton pour ajouter un prefab
        if (GUILayout.Button("Ajouter un Prefab"))
        {
            prefabsToExport.Add(null);
        }

        GUILayout.Space(15);

        // Réglages
        resolution = EditorGUILayout.IntSlider("Résolution PNG", resolution, 256, 4096);
        targetSize = EditorGUILayout.FloatField("Taille uniforme", targetSize);
        outputFolder = EditorGUILayout.TextField("Dossier d'export", outputFolder);

        GUILayout.Space(10);

        GUILayout.Label("Rotation de la caméra :", EditorStyles.boldLabel);
        cameraRotation = EditorGUILayout.Vector3Field("Rotation (X/Y/Z)", cameraRotation);

        GUILayout.Space(20);

        if (GUILayout.Button("Exporter tous les Prefabs"))
        {
            ExportAllPrefabs();
        }
    }

    private void ExportAllPrefabs()
    {
        if (prefabsToExport.Count == 0)
        {
            Debug.LogError("La liste est vide !");
            return;
        }

        SetupCaptureCamera();

        string folderPath = Application.dataPath + "/" + outputFolder;
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        foreach (GameObject prefab in prefabsToExport)
        {
            if (prefab == null) continue;

            ExportSinglePrefab(prefab, folderPath);
        }

        prefabsToExport.Clear();  // 🔥 vide la liste après export

        Debug.Log("Export terminé ! La liste a été vidée.");
    }

    private void ExportSinglePrefab(GameObject prefab, string folderPath)
    {
        // Scène temporaire
        GameObject tempRoot = new GameObject("TempExportRoot");

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, tempRoot.transform);

        // Normalisation des tailles
        NormalizeObjectSize(instance, targetSize);

        // Rotation caméra
        captureCamera.transform.rotation = Quaternion.Euler(cameraRotation);

        // Auto framing
        FrameObject(captureCamera, instance);

        // Export PNG
        string path = folderPath + "/" + prefab.name + ".png";
        CapturePNG(path);

        DestroyImmediate(tempRoot);
    }

    // ---------------------
    //  CAMERA SETUP
    // ---------------------
    private void SetupCaptureCamera()
    {
        if (captureCamera == null)
        {
            GameObject camObj = new GameObject("ExportCamera");
            captureCamera = camObj.AddComponent<Camera>();

            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = new Color(0, 0, 0, 0);
            captureCamera.orthographic = true;
            captureCamera.nearClipPlane = 0.01f;
            captureCamera.farClipPlane = 100f;
        }
    }

    // ---------------------
    //  AUTO-FRAMING
    // ---------------------
    public static void FrameObject(Camera cam, GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        Bounds b = rend.bounds;

        // Position camera selon sa rotation
        cam.transform.position = b.center - cam.transform.forward * 10f;

        float maxSize = Mathf.Max(b.size.x, b.size.y, b.size.z);
        cam.orthographicSize = maxSize * 0.6f;
    }

    // ---------------------
    //  NORMALISATION D’ÉCHELLE
    // ---------------------
    public static void NormalizeObjectSize(GameObject obj, float targetSize)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        Bounds b = rend.bounds;

        float maxDim = Mathf.Max(b.size.x, b.size.y, b.size.z);
        float scaleFactor = targetSize / maxDim;

        obj.transform.localScale *= scaleFactor;
    }

    // ---------------------
    //  CAPTURE PNG
    // ---------------------
    private void CapturePNG(string filePath)
    {
        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);

        captureCamera.targetTexture = rt;
        RenderTexture.active = rt;

        captureCamera.Render();

        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        File.WriteAllBytes(filePath, tex.EncodeToPNG());

        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
    }
}