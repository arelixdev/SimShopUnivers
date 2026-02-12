using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SpriteExporterWindow : EditorWindow
{
    private const int PREVIEW_SIZE = 256;
    // Preview
    private Camera previewCamera;
    private RenderTexture previewRT;
    private GameObject previewInstance;

    // Camera control
    private Vector2 previewRotation = new Vector2(30f, 45f); // X = pitch, Y = yaw
    private float previewDistance = 10f;


    [SerializeField]
    private List<GameObject> prefabsToExport = new List<GameObject>();

    private Camera captureCamera;

    private int resolution = 1024;
    private float targetSize = 1f;
    private string outputFolder = "SpritesExported";

    private int valueTest;

    private Vector3 cameraRotation = new Vector3(0, 0, 0);

    private SerializedObject so;
    private SerializedProperty prefabListProperty;
    private ReorderableList prefabList;

    private Color backgroundColor;

    private bool isTransparent;

    private Texture2D iconFront;
    private Texture2D iconRight;

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

        iconFront = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/SpriteExporter/Icons/iconfront.png");
        iconRight = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/SpriteExporter/Icons/iconright.png");

        backgroundColor = new Color(0,0,0, 1);
    }

    private void OnGUI()
    {
        
        GUILayout.Label("EXPORT LIST OF PREFAB => PNG", EditorStyles.boldLabel);

        GUILayout.Space(10);

        so.Update();

        

        prefabList.DoLayoutList();
        HandleDragAndDrop();

        so.ApplyModifiedProperties();

        if (prefabsToExport.Count == 0 && previewInstance != null)
        {
            ClearPreview();
        }

        if(GUILayout.Button("Clear all prefab"))
        {
            ClearList();
        }

        GUILayout.Space(15);

        resolution = EditorGUILayout.IntSlider("PNG resolution", resolution, 64, 4096);
        //targetSize = EditorGUILayout.FloatField("Taille uniforme", targetSize);
        outputFolder = EditorGUILayout.TextField("Export Folder", outputFolder);

        

        GUILayout.Space(10);

        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);

        GUILayout.Label("Camera Preview", EditorStyles.boldLabel);

        Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE,PREVIEW_SIZE,GUILayout.ExpandWidth(false));

        previewRect.x += 80;

        EditorGUI.DrawRect(previewRect, backgroundColor);

        HandlePreviewMouse(previewRect);
        DrawPreview(previewRect);

        GUILayout.Space(20);

        if (GUILayout.Button("Export"))
        {
            ExportAllPrefabs();
        }

        

        
    }

    private void HandlePreviewMouse(Rect rect)
    {
        Event e = Event.current;

        if (!rect.Contains(e.mousePosition))
            return;

        if (e.type == EventType.MouseDrag && e.button == 0)
        {
            previewRotation.y += e.delta.x;
            previewRotation.x += e.delta.y;

            previewRotation.x = Mathf.Clamp(previewRotation.x, -80f, 80f);

            e.Use();
            Repaint();
        }
    }

    private void DrawPreview(Rect rect)
    {
        if (prefabsToExport.Count == 0 || prefabsToExport[0] == null)
        {
            return;
        }
            

        SetupPreviewCamera();

        if (previewRT == null)
            previewRT = new RenderTexture(PREVIEW_SIZE, PREVIEW_SIZE, 24);

        previewCamera.targetTexture = previewRT;

        ClearRT(previewRT);

        if (previewInstance == null)
            CreatePreviewInstance();

        ApplyCameraTransform(previewCamera, previewInstance);

        previewCamera.Render();

        GUI.DrawTexture(rect, previewRT, ScaleMode.ScaleToFit, true);
    }

    private void CreatePreviewInstance()
    {
        CleanupPreviewInstance();

        previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabsToExport[0]);

        NormalizeObjectSize(previewInstance, targetSize);
    }

    private void ApplyCameraTransform(Camera cam, GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        Bounds b = rend.bounds;

        Quaternion rot = Quaternion.Euler(previewRotation.x, previewRotation.y, 0f);
        cam.transform.rotation = rot;

        cam.transform.position = b.center - cam.transform.forward * previewDistance;

        float maxSize = Mathf.Max(b.size.x, b.size.y, b.size.z);
        cam.orthographicSize = maxSize * 0.6f;
    }

    private void SetupPreviewCamera()
    {
        if (previewCamera != null) return;

        GameObject camObj = new GameObject("PreviewCamera");

        previewCamera = camObj.AddComponent<Camera>();
        previewCamera.orthographic = true;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = backgroundColor;
        previewCamera.nearClipPlane = 0.01f;
        previewCamera.farClipPlane = 100f;
    }

    private void CleanupPreviewInstance()
    {
        if (previewInstance != null)
            DestroyImmediate(previewInstance);
    }

    private void OnDisable()
    {
        CleanupPreviewInstance();

        if (previewCamera != null)
            DestroyImmediate(previewCamera.gameObject);

        if (previewRT != null)
            previewRT.Release();
    }

    private void ClearList()
    {
        prefabsToExport.Clear();  
        
        previewInstance = null;
        ClearPreview();
        ClearAllHideAndDontSave();
    }

    static void ClearAllHideAndDontSave()
    {
        Object[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if ((go.hideFlags & HideFlags.HideAndDontSave) != 0)
            {
                Debug.Log("Destroying: " + go.name);
                Object.DestroyImmediate(go);
            }
        }
    }

    private void ClearPreview()
    {
        if (previewInstance != null)
        {
            DestroyImmediate(previewInstance);
            previewInstance = null;
        }

        if (previewCamera != null)
            previewCamera.targetTexture = null;

        if (previewRT != null)
        {
            ClearRT(previewRT);
            previewRT.Release();
            previewRT = null;
        }

        Repaint();
    }

    private void ClearRT(RenderTexture rt)
    {
        if(rt == null) return;


        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        GL.Clear(true, true, Color.clear);

        RenderTexture.active = previous;
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

        CleanupPreviewInstance();

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
        GameObject tempRoot = new GameObject("TempExportRoot");

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, tempRoot.transform);

        NormalizeObjectSize(instance, targetSize);

        cameraRotation = previewRotation; 

        ApplyCameraTransform(captureCamera, instance);

        string path = folderPath + "/" + prefab.name + ".png";
        CapturePNG(path);

        DestroyImmediate(tempRoot);
    }

    private void SetupCaptureCamera()
    {
        if (captureCamera == null)
        {
            GameObject camObj = new GameObject("ExportCamera");
            captureCamera = camObj.AddComponent<Camera>();

            captureCamera.clearFlags = CameraClearFlags.SolidColor;
            captureCamera.backgroundColor = backgroundColor;
            captureCamera.orthographic = true;
            captureCamera.nearClipPlane = 0.01f;
            captureCamera.farClipPlane = 100f;
        }
    }

    public static void FrameObject(Camera cam, GameObject obj)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        Bounds b = rend.bounds;

        // Position camera selon sa rotation
        cam.transform.position = b.center - cam.transform.forward * 10f;

        float maxSize = Mathf.Max(b.size.x, b.size.y, b.size.z);
        cam.orthographicSize = maxSize * 0.6f;
    }

    public static void NormalizeObjectSize(GameObject obj, float targetSize)
    {
        Renderer rend = obj.GetComponentInChildren<Renderer>();
        Bounds b = rend.bounds;

        float maxDim = Mathf.Max(b.size.x, b.size.y, b.size.z);
        float scaleFactor = targetSize / maxDim;

        obj.transform.localScale *= scaleFactor;
    }

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