using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtTile : MonoBehaviour
{


    private GridManager grid;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse click on tile
        if (Input.GetMouseButtonDown(0) && grid != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(mousePos.x + 0.5f);
            int y = Mathf.FloorToInt(mousePos.y + 0.5f);
            grid.DigTile(x, y);

        }
    }
}
