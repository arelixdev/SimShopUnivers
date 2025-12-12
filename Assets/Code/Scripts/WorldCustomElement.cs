using UnityEngine;

public enum ElementType
{
    Wall,
    Door,
    Window,
    ShopWindow,
    Ground,
    Delete
}

public class WorldCustomElement : MonoBehaviour
{
    public ElementType elementType;
    public GameObject[] listWalls;

    private bool isPainted;

    public void PaintElement(Material mat, int val)
    {
         if (!isPainted)
        {
            isPainted = true;

            if(elementType != ElementType.Ground)
            {
                foreach (GameObject wall in listWalls)
                {
                    if (wall.transform.childCount >= 2)
                    {
                        wall.transform.GetChild(0).gameObject.SetActive(false);
                        wall.transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            } else
            {
                listWalls[0].gameObject.SetActive(false);
                listWalls[1].gameObject.SetActive(true);
            }

            
        }

        // ---- Application des matériaux ----
        foreach (GameObject wall in listWalls)
        {
            Transform t = wall.transform.childCount > 1 ? wall.transform.GetChild(1) : wall.transform;
            MeshRenderer rend = t.GetComponent<MeshRenderer>();
            if (!rend) continue;

            Material[] mats = rend.materials;

            switch (elementType)
            {
                case ElementType.Wall:
                    ApplyWallMaterials(mats, val, mat);
                    break;

                case ElementType.Door:
                    ApplyDoorMaterials(mats, val, mat);
                    break;

                case ElementType.Ground:
                    ApplyGroundMaterials(mats, val, mat);
                    break;
            }

            rend.materials = mats;
        }

        
    }

    private void ApplyWallMaterials(Material[] mats, int val, Material mat)
    {
        if (mats.Length < 2) return;

        if (val == 0 || val == -1)
        {
            mats[0] = mat;
        }
        else if (val == 1)
        {
            mats[1] = mat;
        }
    }

    private void ApplyDoorMaterials(Material[] mats, int val, Material mat)
    {
        if (mats.Length < 3) return;

        // mats[0] = contour (ne jamais modifier)

        if (val == 0 || val == -1)
        {
            mats[0] = mat;
        }
        else if (val == 1)
        {
            mats[2] = mat;
        }else if (val == 2)
        {
            mats[1] = mat;
        }
    }

    private void ApplyGroundMaterials(Material[] mats, int val, Material mat)
    {
        if (mats.Length < 1) return;

        mats[0] = mat; // toujours mat 0
    }
}
