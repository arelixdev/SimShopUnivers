using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopZone : MonoBehaviour
{
    [SerializeField] private string nameShop;
    [SerializeField] private TextMeshPro nameObj;
    [SerializeField] private float textYOffset = 0.5f; 
    [SerializeField] private Color color = Color.yellow;
    private Material mat;
    private SphereCollider sphere;

    private int levelShop = 1;
    private int xpAct = 0;
    private bool playerIn;

    public string GetNameShop()
    {
        return nameShop;
    }

    public void SetNameShop(string nameMod)
    {
        nameShop = nameMod;
        nameObj.text = nameShop.ToUpper();
    }


     void Awake()
    {
        sphere = GetComponent<SphereCollider>();
        mat = new Material(Shader.Find("Hidden/Internal-Colored"));

        if (nameObj != null)
            nameObj.text = nameShop.ToUpper();; 
    }

    void Update()
    {
        if (sphere == null || nameObj == null) return;

        // Rayon réel en tenant compte du scale
        float scaledRadius = sphere.radius * transform.lossyScale.x;

        // Positionnement du texte au-dessus de la sphère
        Vector3 worldCenter = transform.TransformPoint(sphere.center);
        Vector3 newPos = worldCenter + Vector3.up * (scaledRadius + textYOffset);

        nameObj.transform.position = newPos;

        /*if(playerIn && Keyboard.current.pKey.wasPressedThisFrame)
        {
            AddXp(60);
        }*/
    }

    void OnDrawGizmos()
    {
        if (sphere == null) sphere = GetComponent<SphereCollider>();

        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.TransformPoint(sphere.center), sphere.radius * transform.lossyScale.x);
    }

    void OnRenderObject()
    {
        if (mat == null || sphere == null) return;

        mat.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(
            transform.TransformPoint(sphere.center),
            transform.rotation,
            Vector3.one * (sphere.radius * transform.lossyScale.x)
        ));

        GL.Begin(GL.LINES);
        GL.Color(color);

        DrawCircle(Vector3.right, Vector3.up);
        DrawCircle(Vector3.up, Vector3.forward);
        DrawCircle(Vector3.right, Vector3.forward);

        GL.End();
        GL.PopMatrix();
    }

    void DrawCircle(Vector3 axis1, Vector3 axis2)
    {
        int segments = 64;

        for (int i = 0; i < segments; i++)
        {
            float a = (float)i / segments * Mathf.PI * 2f;
            float b = (float)(i + 1) / segments * Mathf.PI * 2f;

            Vector3 p1 = axis1 * Mathf.Cos(a) + axis2 * Mathf.Sin(a);
            Vector3 p2 = axis1 * Mathf.Cos(b) + axis2 * Mathf.Sin(b);

            GL.Vertex(p1);
            GL.Vertex(p2);
        }
    }

    void AddXp(int val)
    {
        xpAct += val;


        if(xpAct >= StoreController.instance.GetXpRequiered()[levelShop-1])
        {
            int diffVal = xpAct - StoreController.instance.GetXpRequiered()[levelShop-1];
            levelShop++;
            xpAct = diffVal;
        }
        UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI(nameShop, levelShop, xpAct);
            playerIn = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            UIController.instance.UpdateShopUI("", 0, 0);
            playerIn = false;
        }
    }
}
