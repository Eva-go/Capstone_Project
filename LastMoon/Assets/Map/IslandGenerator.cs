using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public float scale = 1.0f;
    public int octaves = 3;
    public float persistance = 0.5f;
    public float lacunarity = 2;

    private float xOrg = 0;
    private float yOrg = 0;

    public string seed;
    public bool useRandomSeed;

    public bool useColorMap;
    public bool useGradientMap;

    [SerializeField] private PerlinNoise perlinNoise;
    [SerializeField] private Gradient gradient;
    [SerializeField] private MapDisplay mapDisplay;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (useRandomSeed) seed = Time.time.ToString(); //�õ�
            System.Random pseudoRandom = new System.Random(seed.GetHashCode()); //�ǻ� ����
            xOrg = pseudoRandom.Next(0, 99999); //�ǻ� ������ ���� ���� �� ����
            yOrg = pseudoRandom.Next(0, 99999);
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        //float[,] noiseMap = perlinNoise.GenerateNoiseMap(width, height, scale, octaves, persistance, lacunarity, xOrg, yOrg); //������ �� ����
        //float[,] gradientMap = gradient.GenerateMap(width, height); //�׶���Ʈ �� ����
        //if (useGradientMap) Map2Display.DrawNoiseMap(noiseMap, gradientMap, useColorMap); //������ �ʰ� �׶���Ʈ �� ����
        //else mapDisplay.DrawNoiseMap(noiseMap, noiseMap, useColorMap);
    }
}
