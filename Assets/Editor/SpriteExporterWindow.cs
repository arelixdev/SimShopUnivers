using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteExporterWindow : EditorWindow
{
    [SerializeField]
    private List<GameObject> prefabsToExport = new List<GameObject>();

    private SerializedProperty prefabExport;

    private Camera captureCamera;

    private int resolution = 1024;
    private float targetSize = 1f;
    private string outputFolder = "SpritesExported";

    private Vector3 cameraRotation = new Vector3(0, 0, 0);

    private SerializedObject so;
    private SerializedProperty prefabListProperty;
    private ReorderableList prefabList;

    [MenuItem("Tools/Sprite Exporter")]
    public static void ShowWindow()
    {
        GetWindow<SpriteExporterWindow>("Sprite Exporter");
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        prefabListProperty = so.FindProperty("prefabsToExport");

        prefabList = new ReorderableList(
            so,
            prefabListProperty,
            true,   // draggable
            true,   // display header
            true,   // display add button
            true    // display remove button
        );

        prefabList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "List prefab export");
        };

        prefabList.drawElementCallback = (rect, index, active, focused) =>
        {
            SerializedProperty element = prefabListProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element,
                GUIContent.none
            );
        };

        prefabList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
    }

    private void OnGUI()
    {
        
        GUILayout.Label("EXPORT LIST OF PREFAB => PNG", EditorStyles.boldLabel);

        GUILayout.Space(10);

        so.Update();

        

        prefabList.DoLayoutList();
        HandleDragAndDrop();

        so.ApplyModifiedProperties();

        if(GUILayout.Button("Clear all prefab"))
        {
            ClearList();
        }

        GUILayout.Space(15);

        resolution = EditorGUILayout.IntSlider("PNG resolution", resolution, 64, 4096);
        //targetSize = EditorGUILayout.FloatField("Taille uniforme", targetSize);
        outputFolder = EditorGUILayout.TextField("Export Folder", outputFolder);

        

        GUILayout.Space(10);

        GUILayout.Label("Camera Angle", EditorStyles.boldLabel);
        //ADD icn preset
        GUILayout.Label("Camera Angle Custom", EditorStyles.boldLabel);
        cameraRotation = EditorGUILayout.Vector3Field("Rotation (X/Y/Z)", cameraRotation);

        GUILayout.Space(20);

        if (GUILayout.Button("Export"))
        {
            ExportAllPrefabs();
        }
    }

    private void ClearList()
    {
        prefabsToExport.Clear();  
    }

    private void HandleDragAndDrop()
    {
        Event evt = Event.current;
        Rect listRect = GUILayoutUtility.GetLastRect();

        if (!listRect.Contains(evt.mousePosition))
            return;

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (Object dragged in DragAndDrop.objectReferences)
                {
                    if (dragged is GameObject go &&
                        PrefabUtility.IsPartOfPrefabAsset(go))
                    {
                        prefabListProperty.arraySize++;
                        prefabListProperty.GetArrayElementAtIndex(
                            prefabListProperty.arraySize - 1
                        ).objectReferenceValue = go;
                    }
                }
            }

            evt.Use();
        }
    }

    private void ExportAllPrefabs()
    {
        if (prefabsToExport.Count == 0)
        {
            Debug.LogError("Prefab list is empty");
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

        Debug.Log("Export Finish !");
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