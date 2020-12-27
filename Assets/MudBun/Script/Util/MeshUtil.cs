/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;

using UnityEngine;

namespace MudBun
{
  public class MeshUtil
  {
    public static int EmissionHashUvIndex = 6;
    public static int MetallicSmoothnessUvIndex = 7;

    private static Vector3 Quantize(Vector3 v, float step)
    {
      Vector3 s = new Vector3(Mathf.Sign(v.x), Mathf.Sign(v.y), Mathf.Sign(v.z));
      v += 0.125f * step * Vector3.one;
      v = VectorUtil.CompDiv(v, 0.25f * step * Vector3.one);
      v = VectorUtil.Abs(v);
      v = new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
      v = VectorUtil.CompMul(s, v);
      return v;
    }

    public static void Weld(Mesh mesh)
    {
      var aOldVert = mesh.vertices;
      var aOldNorm = mesh.normals;
      var aOldColor = mesh.colors;
      var aOldBoneWeight = mesh.boneWeights;
      var aOldBindPose = mesh.bindposes;
      var aOldEmissionHash = new List<Vector4>();
      var aOldMetallicSmoothness = new List<Vector2>();
      mesh.GetUVs(EmissionHashUvIndex, aOldEmissionHash);
      mesh.GetUVs(MetallicSmoothnessUvIndex, aOldMetallicSmoothness);

      var aOldIndex = mesh.GetIndices(0);

      var vertToIndexMap = new Dictionary<int, int>();
      var indexToIndexMap = new int[aOldVert.Length];
      for (int i = 0; i < aOldIndex.Length; ++i)
      {
        int index = aOldIndex[i];
        int key = Codec.Hash(Quantize(aOldVert[index], 1e-6f));
        key = Codec.HashConcat(key, Quantize(aOldNorm[index], 1e-3f));

        int newIndex = -1;
        if (!vertToIndexMap.TryGetValue(key, out newIndex))
        {
          newIndex = vertToIndexMap.Count;
          vertToIndexMap.Add(key, newIndex);

          // debugger-friendly duplicate code
          indexToIndexMap[i] = newIndex;
        }
        else
        {
          // debugger-friendly duplicate code
          indexToIndexMap[i] = newIndex;
        }
      }

      int numUniqueVerts = vertToIndexMap.Count;
      var aNewVert = new Vector3[numUniqueVerts];
      var aNewNorm = new Vector3[numUniqueVerts];
      var aNewColor = new Color[numUniqueVerts];
      var aNewEmissionHash = new Vector4[numUniqueVerts];
      var aNewMetallicSmoothness = new Vector2[numUniqueVerts];
      var aNewBoneWeight = new BoneWeight[numUniqueVerts];
      var aNewBindPose = aOldBindPose; // bind poses aren't changed
      for (int oldIndex = 0; oldIndex < indexToIndexMap.Length; ++oldIndex)
      {
        int newIndex = indexToIndexMap[oldIndex];
        aNewVert[newIndex] = aOldVert[oldIndex];
        aNewNorm[newIndex] = aOldNorm[oldIndex];
        aNewColor[newIndex] = aOldColor[oldIndex];
        aNewEmissionHash[newIndex] = aOldEmissionHash[oldIndex];
        aNewMetallicSmoothness[newIndex] = aOldMetallicSmoothness[oldIndex];

        if (aOldBoneWeight != null && aOldBoneWeight.Length >= aOldVert.Length)
          aNewBoneWeight[newIndex] = aOldBoneWeight[oldIndex];
      }

      var aNewIndex = new int[aOldIndex.Length];
      for (int i = 0; i < aOldIndex.Length; ++i)
      {
        aNewIndex[i] = indexToIndexMap[aOldIndex[i]];
      }

      var topology = mesh.GetTopology(0);
      mesh.Clear();
      mesh.SetVertices(aNewVert);
      mesh.SetNormals(aNewNorm);
      mesh.SetColors(aNewColor);
      mesh.boneWeights = aNewBoneWeight;
      mesh.bindposes = aNewBindPose;
      mesh.SetUVs(EmissionHashUvIndex, aNewEmissionHash);
      mesh.SetUVs(MetallicSmoothnessUvIndex, aNewMetallicSmoothness);
      mesh.SetIndices(aNewIndex, topology, 0);
    }
  }
}

