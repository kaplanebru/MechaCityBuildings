using UnityEngine;

public class MapSizeToGridSize : MonoBehaviour
{
   public Transform plane;

   public Vector2Int GetToGridSizeFromMesh()
   {
      Vector2Int size = new();

      var planeX = plane.localScale.x;
      var planeZ = plane.localScale.z;

      size.x = Mathf.RoundToInt(planeX * transform.localScale.x * 10);
      size.y = Mathf.RoundToInt(planeZ * transform.localScale.z * 10);
      
      print(size);
      return size;
   }
}
