using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardStore : MySingleton<CardStore>
{
    public Shader RectShader;
    public List<Material> CardList = new() { };
  
    public void GetAllCardImages()
    { 
        Object[] textures = Resources.LoadAll("CardImages", typeof(Texture2D));

        foreach (Object _tex in textures) {
            //string path = AssetDatabase.GUIDToAssetPath(guid);
            //if (string.IsNullOrEmpty(path)) continue;

            //Texture2D pic = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            Material mat = new(RectShader);
            mat.SetTexture("_CardImage", _tex as Texture2D);
            this.CardList.Add(mat);

        }
    }
    public Material GetMaterialByIdx(int i) {  return CardList[i]; }

    public List<Material> GetRandMaterials(int n)
    {
        List<Material> shuffled = new(CardList);
        if (n >= CardList.Count)
        {
            return shuffled;
        }
        int nShuffles = n;
        for (int i = 0; i < nShuffles; i++)
        {
            int j = Random.Range(i, shuffled.Count);
            (shuffled[j], shuffled[i]) = (shuffled[i], shuffled[j]);
        }

        return shuffled.GetRange(0, n);

    }
}

public static class ListExtensions
{

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n-1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}