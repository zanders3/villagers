
using UnityEngine;

public enum Tile
{
    Water,
    Ground
}

[System.Serializable]
public class Level
{
    public const int SubGridSize = 4;
    public static float SubGridScale = 1.0f / SubGridSize;
    
    [SerializeField] private int width = 0, height = 0, subwidth = 0, subheight = 0;
    [SerializeField] private Tile[] tiles = new Tile[0];
    [SerializeField] private bool[] isWalkable = new bool[0];
    
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int SubWidth { get { return subwidth; } }
    public int SubHeight { get { return subheight; } }
    
    public Tile this[int x, int y]
    {
        get 
        { 
            if (x >= 0 && x < width && y >= 0 && y < height && tiles.Length > 0)
                return tiles[(x * height) + y];
            else
                return Tile.Water;
        }
        set { tiles[(x * height) + y] = value; }
    }
    
    public void Resize(int newWidth, int newHeight)
    {
        Debug.Log("Resize: " + newWidth + " x " + newHeight);
        Tile[] newTiles = new Tile[newWidth * newHeight];
        int mx = Mathf.Min(newWidth, width), my = Mathf.Min(newHeight, height);
        Debug.Log("Resize: " + mx + " x " + my);
        for (int x = 0; x<mx; x++)
            for (int y = 0; y<my; y++)
                newTiles[(x * newHeight)+y] = this[x,y];
        
        this.tiles = newTiles;
        this.width = newWidth;
        this.height = newHeight;
        this.subwidth = newWidth * SubGridSize;
        this.subheight = newHeight * SubGridSize;
        this.isWalkable = new bool[subwidth * subheight];
        
        for (int x = 0; x<subwidth; x++)
            for (int y = 0; y<subheight; y++)
                isWalkable[(x*subheight) + y] = this[x/SubGridSize, y/SubGridSize] == Tile.Ground;
    }
    
    public bool IsWalkable(int subx, int suby)
    {
        return isWalkable[subx*subheight + suby];
    }
}

