using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerate : MonoBehaviour
{
    [SerializeField] private PerlinNoise perlinNoise;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Map2Display map2Display;

    [Header("Map Parameters")]
    public int mapWidth = 1000;
    public int mapHeight = 1000;
    public float noiseScale = 10f;
    public int octaves = 4;

    public string seed;
    public bool useRandomSeed;

    public bool useColorMap;
    public bool useGradientMap;

    [Range(0f, 1f)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public float xOrg = 0f;
    public float yOrg = 0f;

    private void Start()
    {
        GenerateMap();
    }

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

    public void GenerateMap()
    {
        // Generate noise map
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, xOrg, yOrg);

        // Generate gradient map
        float[,] gradientMap = gradient.GenerateMap(mapWidth, mapHeight);

        if (useGradientMap) map2Display.DrawNoiseMap(noiseMap, gradientMap, useColorMap); //������ �ʰ� �׶���Ʈ �� ����
        else map2Display.DrawNoiseMap(noiseMap, noiseMap, useColorMap);
    }
}