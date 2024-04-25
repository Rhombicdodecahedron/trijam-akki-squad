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
    // end screen
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



    public enum TileType
    {
        Dirt,
        DiggedDirt
    }


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

    // Start is called before the first frame update
    void Start()
    {
        print("GridManager Start");
        tiles = new GameObject[width, height];
        initParameters();
        GenerateGrid();

        // Center camera
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

                // Randomly flip for variety
                if (Random.value < 0.5f) newTile.GetComponent<SpriteRenderer>().flipX = true;

                tiles[x, y] = newTile;

                // Randomly place a bone
                RandomlyPlaceBone(x, y, newTile);
                RandomlyPlaceBomb(x, y, newTile);
            }
        }

    }

    private void RandomlyPlaceBomb(int x, int y, GameObject newTile)
    {
        if (Random.value < 0.2f)
        {
            print("Bomb placed");
            // Instantiate the bomb prefab and set its sorting order higher than the tile
            GameObject bombPrefab = tilePrefabs[4];
            GameObject newBomb = Instantiate(bombPrefab, new Vector3(x, y), Quaternion.identity);
            newBomb.name = $"Bomb {x},{y}";

            bombs--;
        }
    }

    private void RandomlyPlaceBone(int x, int y, GameObject newTile)
    {
        // Randomly determine whether to place a bone
        if (Random.value < 0.5f)
        {
            print("Bone placed");
            // Instantiate the bone prefab and set its sorting order higher than the tile
            GameObject bonePrefab = tilePrefabs[Random.Range(2, 4)];
            GameObject newBone = Instantiate(bonePrefab, new Vector3(x, y), Quaternion.identity);
            newBone.name = $"Bone {x},{y}";

            bones--;
        }
    }

    public void DigTile(int x, int y)
    {

        if (isGameOver) return;

        // Play dig sound
        audioSource.clip = digSound[Random.Range(0, digSound.Length)];
        audioSource.Play();

        print($"DigTile {x},{y}");
        Destroy(tiles[x, y]);

        // Instantiate the tile prefab
        GameObject tilePrefab = tilePrefabs[1];
        GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
        newTile.name = $"Tile {x},{y}";
        tiles[x, y] = newTile;



        // show the bone
        GameObject newBone = GameObject.Find($"Bone {x},{y}");

        // check if there is a bomb
        GameObject newBomb = GameObject.Find($"Bomb {x},{y}");
        if (newBone != null)
        {
            newBone.SetActive(true);

            // Get the SpriteRenderer components of both the tile and bone
            SpriteRenderer tileRenderer = newTile.GetComponent<SpriteRenderer>();
            SpriteRenderer boneRenderer = newBone.GetComponent<SpriteRenderer>();

            // Ensure the bone appears on top of the tile by setting its sorting order higher
            boneRenderer.sortingOrder = tileRenderer.sortingOrder + 1;

            // Play brush sound
            audioSource.clip = brushSound;
            audioSource.Play();

            // Increase score
            score++;

        }
        else if (newBomb != null)
        {
            print("Bomb exploded");
            newBomb.SetActive(true);

            // Explode the bomb
            // ExplodeBomb(x, y);

            // Get the SpriteRenderer components of both the tile and bone
            SpriteRenderer tileRenderer = newTile.GetComponent<SpriteRenderer>();

            SpriteRenderer bombRenderer = newBomb.GetComponent<SpriteRenderer>();

            // Ensure the bone appears on top of the tile by setting its sorting order higher
            bombRenderer.sortingOrder = tileRenderer.sortingOrder + 1;


            // Play bomb sound
            audioSource.clip = bombSound[Random.Range(0, bombSound.Length)];
            audioSource.Play();

            // End game
            endScreen.enabled = true;
            isGameOver = true;
        }





    }

}
