using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour
{
    //private Color[] Colors = new Color[4] { Color.red, Color.blue, Color.green, Color.yellow };
    public Color newColor = new Color(1f, 0f, 0f);
    public int index;

    void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        //index = Random.Range(0, Colors.Length);
        //spriteRenderer.color = Colors[index];
        spriteRenderer.color = newColor;
    }
}