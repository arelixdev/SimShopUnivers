using UnityEngine;

public class PaintCan : MonoBehaviour
{
    public int numberUsage = 5;

    public bool UsePaintCan()
    {
        if( numberUsage > 0)
        {

            numberUsage--;
            return true;
        } else
        {
            return false;
        }
    }
}
