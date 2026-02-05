using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridOptimizer
{
   /* private List<Vector3> trackedCells = new();

    private Vector3 GetCenter()
    {
        Vector3 sum = new Vector3();
        foreach (var cell in trackedCells)
        {
            sum += cell;
        }

        sum /= trackedCells.Count;
        Debug.Log(sum);
        return sum;
    }

    static int DirKey2D(Vector3 delta, int bins = 720) // 0.5° çözünürlük
    {
        float a = Mathf.Atan2(delta.z, delta.x); // -pi..pi
        float t = (a + Mathf.PI) / (2f * Mathf.PI); // 0..1
        int k = Mathf.RoundToInt(t * bins) % bins;
        return k;
    }
 
    int AngleFromDir(Vector3 delta)
    {
        float angle = Mathf.Atan2(delta.z, delta.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        if (angle % 2 == 0)
            angle++;
        
        return Mathf.RoundToInt(angle);
    }



    Dictionary<int, List<Vector3>> edgesByDir = new();

    private Vector3 center;

    private void FindEdges() 
    {
        center = GetCenter(); //todo: tam ortası değil de center of mass gibi bir şey lazım L şekli için. L'nin ortası Hipotenüzste boşlukya kalıyor
        edgesByDir.Clear();

        foreach (var cell in trackedCells)
        {
            Vector3 direction = (cell - center).normalized;
            var key = DirKey2D(direction, 180);
            Debug.Log(key);

            if (edgesByDir.ContainsKey(key))
            {
                if (!edgesByDir[key].Contains(cell))
                    edgesByDir[key].Add(cell);
            }
            else
            {
                edgesByDir.Add(key, new List<Vector3>());

                edgesByDir[key].Add(cell);
            }
        }

        var keys = edgesByDir.Keys.ToList(); // snapshot

        foreach (var key in keys)
        {
            edgesByDir[key] = edgesByDir[key]
                .OrderByDescending(x => Vector3.Distance(x, center))
                .ToList();
        }
    }

    public List<Vector3> GetEdges(List<Vector3> cells, int amount)
    {
        trackedCells = cells;
        if (amount <= 0)
        {
            amount = 1;
        }

        FindEdges();
        List<Vector3> edgeCells = new List<Vector3>();

        foreach (var edges in edgesByDir.Values)
        {
            int edgeAmount = edges.Count < amount ? edges.Count : amount;
            for (var i = 0; i < edgeAmount; i++)
            {
                var edge = edges[i];
                edgeCells.Add(edge);
            }
        }

        //edgeCells.Add(center);
        return edgeCells;
    }*/
}