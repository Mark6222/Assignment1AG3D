using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting.Dependencies.Sqlite;
using System.Data;
using UnityEngine.UI;
public class GenerateMaze : MonoBehaviour
{
    public GameObject wall, Mob, Team, t, smallWall, smallWater, smallTree;
    Color[,] colorOfPixel;
    public Texture2D outLineImage;
    int i, j
;
    int[,] worldMap = new int[,]{
        {1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,1,1,1,1,0,0,0,1},
        {1,0,0,0,3,0,0,2,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,0,1,1,1,1,0,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1},
    };
    int column, row;
    GameObject t2;
    void Start()
    {
        // GenerateFromFile();
        GenerateFromImage();
    }
    void GenerateFromImage()
    {
        colorOfPixel = new Color[outLineImage.height, outLineImage.height];
        for (int x = 0; x < outLineImage.width; x++)
        {
            for (int y = 0; y < outLineImage.height; y++)
            {
                colorOfPixel[x, y] = outLineImage.GetPixel(x, y);
                float r, g, b;
                r = colorOfPixel[x, y].r;
                g = colorOfPixel[x, y].g;
                b = colorOfPixel[x, y].b;
                if (colorOfPixel[x, y] == Color.black)
                {
                    t = (GameObject)(Instantiate(smallWall, new Vector3((outLineImage.width / 2) - x, 1.5f, (outLineImage.height / 2) - y), Quaternion.identity));
                    Debug.Log($"Spawned smallWall");

                }
                else if (colorOfPixel[x, y] == Color.green)
                {
                    t = (GameObject)(Instantiate(smallTree, new Vector3((outLineImage.width / 2) - x, 1.5f, (outLineImage.height / 2) - y), Quaternion.identity));
                    Debug.Log($"Spawned smallTree");
                }
                else if (colorOfPixel[x, y] == Color.blue)
                {
                    t = (GameObject)(Instantiate(Team, new Vector3((outLineImage.width / 2) - x, 1.5f, (outLineImage.height / 2) - y), Quaternion.identity));
                    Debug.Log($"Spawned Team");
                }
                else if (colorOfPixel[x, y] == Color.red)
                {
                    t = (GameObject)(Instantiate(Mob, new Vector3((outLineImage.width / 2) - x, 1.5f, (outLineImage.height / 2) - y), Quaternion.identity));
                    Debug.Log($"Spawned Mob");
                }
            }
        }
    }
    // void GenerateFromFile()
    // {
    //     TextAsset t1 = (TextAsset)Resources.Load("maze", typeof(TextAsset));
    //     string s = t1.text;
    //     int i;
    //     s = s.Replace(System.Environment.NewLine, "");
    //     for (i = 0; i < s.Length; i++)
    //     {
    //         column = i % 10;
    //         row = i / 10;
    //         if (s[i] == '1')
    //         {
    //             t = (GameObject)(Instantiate(wall, new Vector3(50 - column * 10, 1.5f, 50 - row * 10), Quaternion.identity));
    //         }
    //         else if (s[i] == '2')
    //         {
    //             Instantiate(target, new Vector3(50 - i * 10, 1.5f, 50 - j * 10), Quaternion.identity);
    //         }
    //         else if (s[i] == '3')
    //         {
    //             Instantiate(npc, new Vector3(50 - i * 10, 1.5f, 50 - j * 10), Quaternion.identity);
    //         }
    //     }
    // }
    // void GenerateFromArray()
    // {
    //     for (int i = 0; i < 10; i++)
    //     {
    //         for (int j = 0; j < 10; j++)
    //         {
    //             if (worldMap[i, j] == 1)
    //             {
    //                 t = (GameObject)(Instantiate(wall, new Vector3(50 - i * 10, 1.5f, 50 - j * 10), Quaternion.identity));
    //             }
    //             else if (worldMap[i, j] == 2)
    //             {
    //                 Instantiate(target, new Vector3(50 - i * 10, 1.5f, 50 - j * 10), Quaternion.identity);
    //             }
    //             else if (worldMap[i, j] == 3)
    //             {
    //                 Instantiate(npc, new Vector3(50 - i * 10, 1.5f, 50 - j * 10), Quaternion.identity);
    //             }
    //         }
    //     }
    // }
    void Update()
    {

    }
}
