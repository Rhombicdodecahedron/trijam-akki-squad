using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public AudioClip[] digSound;
    public AudioClip[] bombSound;
    public AudioClip brushSound;
    private AudioSource audioSource;
    public TMP_Text endScreen;
    public bool isGameOver = false;
    public int score = 0;
    public GameObject[,] tiles;
    public GameObject[] tilePrefabs;
    [SerializeField] private Transform cam;
    public int width = 10;
    public int height = 10;
    public enum LevelParameters
    {
        Easy,
        Medium,
        Hard
    }
    public LevelParameters levelParameters = LevelParameters.Hard;
    private int bones = 0;
    private int bombs = 0;

    private void initParameters()
    {
        switch (levelParameters)
        {
            case LevelParameters.Easy:
                bones = 20;
                bombs = 10;
                break;
            case LevelParameters.Medium:
                bones = 20;
                bombs = 10;
                break;
            case LevelParameters.Hard:
                bones = 20;
                bombs = 15;
                break;
            default:
                bones = 20;
                bombs = 10;
                break;
        }
    }

    void Start()
    {
        print("GridManager Start");
        tiles = new GameObject[width, height];
        initParameters();
        GenerateGrid();
        cam.transform.position = new Vector3(width / 2 - 0.5f, height / 2 - 0.5f, -10);
        audioSource = GetComponent<AudioSource>();
        endScreen.enabled = false;
    }

    public void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tilePrefab = tilePrefabs[0];
                GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                newTile.name = $"Tile {x},{y}";
                if (Random.value < 0.5f) newTile.GetComponent<SpriteRenderer>().flipX = true;
                tiles[x, y] = newTile;
            }
        }
        PlaceBombsAndBones();
    }

    private void PlaceBombsAndBones()
    {
        while (bones > 0 || bombs > 0)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            if (tiles[x, y] != null && GameObject.Find($"Bone {x},{y}") == null && GameObject.Find($"Bomb {x},{y}") == null)
            {
                if (bones > 0 && Random.value < 0.5f)
                {
                    int boneType = Random.Range(2, 4);
                    GameObject bonePrefab = tilePrefabs[boneType];
                    GameObject newBone = Instantiate(bonePrefab, new Vector3(x, y), Quaternion.identity);
                    newBone.name = $"Bone {x},{y}";
                    // hide behind the tile
                    newBone.GetComponent<SpriteRenderer>().sortingOrder = tiles[x, y].GetComponent<SpriteRenderer>().sortingOrder - 1;
                    bones--;
                }
                else if (bombs > 0 && Random.value < 0.2f)
                {
                    GameObject bombPrefab = tilePrefabs[4];
                    GameObject newBomb = Instantiate(bombPrefab, new Vector3(x, y), Quaternion.identity);
                    newBomb.name = $"Bomb {x},{y}";
                    newBomb.GetComponent<SpriteRenderer>().sortingOrder = tiles[x, y].GetComponent<SpriteRenderer>().sortingOrder - 1;
                    bombs--;
                }
            }
        }
    }

    public void DigTile(int x, int y)
    {
        if (isGameOver) return;
        audioSource.clip = digSound[Random.Range(0, digSound.Length)];
        audioSource.Play();
        print($"DigTile {x},{y}");
        Destroy(tiles[x, y]);
        GameObject tilePrefab = tilePrefabs[1];
        GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
        newTile.name = $"Tile {x},{y}";
        tiles[x, y] = newTile;
        GameObject newBone = GameObject.Find($"Bone {x},{y}");
        GameObject newBomb = GameObject.Find($"Bomb {x},{y}");
        if (newBone != null)
        {
            print("Bone found");
            newBone.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
            audioSource.clip = brushSound;
            audioSource.Play();
            score++;
        }
        else if (newBomb != null)
        {
            print("Bomb exploded");
            newBomb.GetComponent<SpriteRenderer>().sortingOrder = newTile.GetComponent<SpriteRenderer>().sortingOrder + 1;
            audioSource.clip = bombSound[Random.Range(0, bombSound.Length)];
            audioSource.Play();
            endScreen.enabled = true;
            isGameOver = true;
        }
    }
}
