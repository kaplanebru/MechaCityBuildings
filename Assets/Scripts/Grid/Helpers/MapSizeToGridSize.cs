using UnityEngine;

public class MapSizeToGridSize : MonoBehaviour
{
   [SerializeField] private Transform plane;
   [SerializeField] private Transform cityBuilder; //to prevent bugs
   public Vector2Int GetGridSizeFromMesh()
   {
      Vector2Int size = new()
      {
          x = Mathf.RoundToInt(plane.localScale.x 
                               * transform.localScale.x 
                               * cityBuilder.localScale.x 
                               * 10),
          y = Mathf.RoundToInt(plane.localScale.z 
                               * transform.localScale.z 
                               * cityBuilder.localScale.z 
                               * 10)
      };

      print(size);
      return size;
   }
}
