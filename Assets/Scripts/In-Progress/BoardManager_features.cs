using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager_features : MonoBehaviour
{
    public RectTransform topPanelParent;
    public RectTransform bottomPanelParent;

    private int screens;
    public int colsPerScreen = 32; //TODO: Figure this out
    public int rowsPerPanel = 7;

    private List<Vector2> emptyPositions = new List<Vector2>();
    private Dictionary<Vector2, RectTransform> blockGrid = new Dictionary<Vector2, RectTransform>();
    private Feature[] possibleFeatures = {
        new Wall(),
        //new Maze(),
        new SupplyCache(),
        //new ZigZag(),
        //new Snake(),
    };
    private Dictionary<Vector2, Feature> features = new Dictionary<Vector2, Feature>();

    public Panel topPanel;
    public Panel bottomPanel;

    public Player topPlayer;
    public Player bottomPlayer;

    //Arrays are for if we want multiple variations of block sprites
    public RectTransform[] goals;
    public RectTransform[] dirtBlocks;
    public RectTransform[] rockBlocks;
    public Count rockCount = new Count(100, 150);
    public RectTransform[] brickBlocks;
    public Count brickCount = new Count(80, 120);
    public RectTransform[] ironBlocks;
    public Count ironCount = new Count(60, 900);
    public RectTransform[] oil;
    public Count oilCount = new Count(20, 40);
    public RectTransform[] tnt;
    public Count tntCount = new Count(20, 40);

    [Serializable]
    public class Count
    {
        public int min;
        public int max;

        public Count(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public abstract class Feature
    {
        public string name;
        public int minCols;
        public int maxCols;
        public int weight;
        public Dictionary<string, RectTransform[]> blocksToUse;
        public int colsToUse = 0;
        public int height;
        public Dictionary<Vector2, RectTransform> blockGrid;

        public Feature(string name, int minCols, int maxCols, int weight)
        {
            this.name = name;
            this.minCols = minCols;
            this.maxCols = maxCols;
            this.weight = weight;
        }

        public void SetColsToUse(int cols)
        {
            this.colsToUse = cols;
        }

        public void SetBlocksToUse(Dictionary<string, RectTransform[]> blocks)
        {
            this.blocksToUse = blocks;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public void SetGrid(Dictionary<Vector2, RectTransform> grid)
        {
            this.blockGrid = grid;
        }

        public abstract void Generate(float col);
    }

    public class Wall : Feature
    {
        public Wall() : base("wall", 2, 2, 1) { }

        public override void Generate(float col)
        {
            RectTransform block = null;
            int blockToUse = Random.Range(0, 2);
            switch(blockToUse)
            {
                case 0:
                    block = blocksToUse["rocks"][0];
                    break;
                case 1:
                    block = blocksToUse["bricks"][0];
                    break;
            }
            for (float x = col; x < col + colsToUse; x++)
            {
                for (float y = 0; y < height; y++)
                {
                    blockGrid.Add(new Vector2(x, y), block);
                }
            }
        }
    }

    public class Maze : Feature
    {
        public Maze() : base("maze", 7, 20, 5) { }

        public override void Generate(float col)
        {

        }
    }

    public class SupplyCache : Feature
    {
        public SupplyCache() : base("cache", 4, 5, 3) { }

        public override void Generate(float col)
        {
            RectTransform dirt = blocksToUse["dirt"][0];
            RectTransform brick = blocksToUse["bricks"][0];
            RectTransform tnt = blocksToUse["tnt"][0];
            RectTransform oil = blocksToUse["oil"][0];
            float bottom;
            float top;
            float middle = height / 2f;
            if (middle % 0.5 == 0)
            {
                bottom = middle - 2.5f;
                top = middle + 2.5f;
            }
            else
            {
                top = middle + 1;
                bottom = middle - 2;
            }
            for (float x = col; x < col + colsToUse; x++)
            {
                for (float y = 0; y < height; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    if (y == bottom || y == top || x == col || x == col + colsToUse)
                    {
                        blockGrid.Add(pos, brick);
                    }
                    else if (bottom < y && y < top)
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            blockGrid.Add(pos, tnt);
                        }
                        else
                        {
                            blockGrid.Add(pos, oil);
                        }
                    }
                    else
                    {
                        blockGrid.Add(pos, dirt);
                    }
                }
            }
        }
    }

    public class ZigZag : Feature
    {
        public ZigZag() : base("zigzag", 9, 15, 3) { }

        public override void Generate(float col)
        {

        }
    }

    public class Snake : Feature
    {
        public Snake() : base("snake", 6, 6, 3) { }

        public override void Generate(float col)
        {

        }
    }

    public class Panel
    {
        public string id;
        public RectTransform parent;
        public int currentScreen;
        public Player player;

        protected Transform panelHolder;
        public Dictionary<Vector2, RectTransform> blocks;

        public Panel(RectTransform parent, Player player)
        {
            this.parent = parent;
            this.player = player;
            this.blocks = new Dictionary<Vector2, RectTransform>();
        }

        public void SetupBlocks(Dictionary<Vector2, RectTransform> map)
        {
            foreach (Vector2 loc in map.Keys)
            {
                Vector2 pos = new Vector2(loc.x + 0.5f, loc.y + 0.5f);
                RectTransform block = Instantiate(map[loc]);
                block.transform.SetParent(parent);
                block.anchoredPosition = pos;
                blocks.Add(pos, block);
            }
        }

        public void Shift()
        {
            Dictionary<Vector2, RectTransform> tempDict = new Dictionary<Vector2, RectTransform>();
            foreach (Vector2 loc in blocks.Keys)
            {
                Vector2 newPos = new Vector2(loc.x - 32, loc.y);
                RectTransform block = blocks[loc];
                block.anchoredPosition = newPos;
                tempDict.Add(newPos, block);
            }
            blocks = tempDict;
        }

        public RectTransform GetBlock(float x, float y)
        {
            foreach (Vector2 loc in blocks.Keys)
            {
                if(loc.x == x && loc.y == y)
                {
                    return blocks[loc];
                }
            }
            return null;
        }

        public Vector2 GetBlockLocation(float x, float y)
        {
            foreach (Vector2 loc in blocks.Keys)
            {
                if (loc.x == x && loc.y == y)
                {
                    return loc;
                }
            }
            return new Vector2(-1, -1);
        }

        public void RemoveBlock(Vector2 loc)
        {
            blocks.Remove(loc);
        }

        public void SetBlockLocation(Vector2 loc, RectTransform block)
        {
            blocks.Remove(loc);
            block = Instantiate(block);
            block.transform.SetParent(parent);
            block.anchoredPosition = loc;
            blocks.Add(loc, block);
        }

        public RectTransform GetRandomBlockOnScreen()
        {
            int x = Random.Range(0, 32);
            int y = Random.Range(0, 7);
            while(GetBlock(x + 0.5f, y + 0.5f) == null)
            {
                x = Random.Range(0, 32);
                y = Random.Range(0, 7);
            }
            return GetBlock(x + 0.5f, y + 0.5f);
        }
    }

    private Transform boardHolder;

    void InitBlockGrid()
    {
        emptyPositions.Clear();
        for (int x = 0; x < colsPerScreen * screens; x++)
        {
            for (int y = 0; y < rowsPerPanel; y++)
            {
                emptyPositions.Add(new Vector2(x, y));
            }
        }
    }

    Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, emptyPositions.Count);
        Vector2 randomPosition = emptyPositions[randomIndex];
        emptyPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutBlockAtRandom(RectTransform[] blockArr, int min, int max)
    {
        int count = Random.Range(min, max + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPosition = RandomPosition();
            RectTransform block = blockArr[Random.Range(0, blockArr.Length)];
            blockGrid.Add(randomPosition, block);
        }
    }

    void CreateGoal()
    {
        float left = colsPerScreen * screens - 6;
        float bottom;
        float top;
        float middle = rowsPerPanel / 2f;
        if (middle % 0.5 == 0)
        {
            bottom = middle - 2.5f;
            top = middle + 2.5f;
        }
        else
        {
            top = middle + 1;
            bottom = middle - 2;
        }
        for (float x = left; x < left + 5; x++)
        {
            for (float y = bottom; y < top; y++)
            {
                Vector2 pos = new Vector2(x, y);
                emptyPositions.Remove(pos);
                if (x == left || y == bottom || y == bottom + 4)
                {
                    blockGrid.Add(pos, ironBlocks[0]);
                }
                else if (x == left + 1 && bottom < y && y < top)
                {
                    blockGrid.Add(pos, goals[0]);
                }
                else
                {
                    blockGrid.Add(pos, dirtBlocks[0]);
                }
            }
        }
    }

    void GenerateFeatures()
    {
        int[][] cols = GetFreeColumns();
        SetFeatureBlocks();
        SetFeatureFields();
        SelectFeatures(cols);
        foreach(Vector2 key in features.Keys)
        {
            features[key].Generate(key.x);
        }
    }

    void SetFeatureBlocks()
    {
        for (int i = 0; i < possibleFeatures.Length; i++)
        {
            Dictionary<string, RectTransform[]> blocksToUse = new Dictionary<string, RectTransform[]>();
            Feature feat = possibleFeatures[i];
            string name = feat.name;
            switch(name)
            {
                case "wall":
                    blocksToUse.Add("rocks", rockBlocks);
                    blocksToUse.Add("bricks", brickBlocks);
                    break;
                case "cache":
                    blocksToUse.Add("tnt", tnt);
                    blocksToUse.Add("oil", oil);
                    blocksToUse.Add("bricks", brickBlocks);
                    break;
                default:
                    blocksToUse.Add("bricks", brickBlocks);
                    break;
            }
            blocksToUse.Add("dirt", dirtBlocks);
            feat.SetBlocksToUse(blocksToUse);
        }
    }

    void SetFeatureFields()
    {
        for (int i = 0; i < possibleFeatures.Length; i++)
        {
            possibleFeatures[i].SetHeight(rowsPerPanel);
            possibleFeatures[i].SetGrid(blockGrid);
        }
    }

    void SelectFeatures(int[][] cols)
    {
        int[] numFeats = new int[screens];
        int smallestWidth = colsPerScreen + 1;
        for (int i = 0; i < possibleFeatures.Length; i++)
        {
            int minWidth = possibleFeatures[i].minCols;
            if (minWidth < smallestWidth)
            {
                smallestWidth = minWidth;
            }
        }
        for (int i = 0; i < screens; i++)
        {
            int freeCols = 32;
            for (int j = 0; j < colsPerScreen; j++)
            {
                if (cols[i][j] == 1)
                    freeCols--;
            }
            int count = 0;
            while (numFeats[i] < 2 && freeCols > smallestWidth && count<10)
            {
                int space = Random.Range(1, 4);
                freeCols -= space;
                List<Feature> feats = new List<Feature>();
                for (int j = 0; j < possibleFeatures.Length; j++)
                {
                    if (possibleFeatures[j].minCols <= freeCols)
                    {
                        feats.Add(possibleFeatures[j]);
                    }
                }
                Feature selected = feats[Random.Range(0, feats.Count)];
                int colsToUse = Math.Min(selected.maxCols, freeCols);
                selected.SetColsToUse(colsToUse);
                int startingCol = i * screens + 32 - freeCols;
                for (int x = startingCol; x < startingCol + colsToUse; x++)
                {
                    for (int y = 0; y < rowsPerPanel; y++)
                    {
                        emptyPositions.Remove(new Vector2(x, y));
                    }
                }
                features.Add(new Vector2(startingCol, 0), selected);
                freeCols -= colsToUse;
                numFeats[i]++;
                Debug.Log("free columns: " + freeCols);
                count++;
            }
        }
    }

    int[][] GetFreeColumns()
    {
        int[][] columns = new int[screens][];
        for (int i = 0; i < screens; i++)
        {
            columns[i] = new int[colsPerScreen];
            columns[i][0] = 1;
        }
        for (int i = colsPerScreen - 7; i < colsPerScreen; i++)
        {
            columns[screens - 1][i] = 1;
        }
        return columns;
    }

    void FillWithDirt()
    {
        while (emptyPositions.Count > 0)
        {
            Vector2 pos = emptyPositions[0];
            emptyPositions.RemoveAt(0);
            RectTransform block = dirtBlocks[Random.Range(0, dirtBlocks.Length)];
            blockGrid.Add(pos, block);
        }
    }

    public void Setup(int numScreens)
    {
        screens = numScreens;
        topPanel = new Panel(topPanelParent, topPlayer);
        bottomPanel = new Panel(bottomPanelParent, bottomPlayer);
        InitBlockGrid();
        CreateGoal();
        GenerateFeatures();
        LayoutBlockAtRandom(rockBlocks, rockCount.min, rockCount.max);
        LayoutBlockAtRandom(brickBlocks, brickCount.min, brickCount.max);
        LayoutBlockAtRandom(ironBlocks, ironCount.min, ironCount.max);
        LayoutBlockAtRandom(oil, oilCount.min, oilCount.max);
        LayoutBlockAtRandom(tnt, tntCount.min, tntCount.max);
        FillWithDirt();
        topPanel.SetupBlocks(blockGrid);
        bottomPanel.SetupBlocks(blockGrid);
        //RectTransform topPlayerStart = GetBlock(0.5f, 3.5f, "top");
        //RectTransform bottomPlayerStart = GetBlock(0.5f, 3.5f, "bottom");
        RectTransform topPlayerStart = GetBlock(0.5f, -3.5f);
        RectTransform bottomPlayerStart = GetBlock(0.5f, -14.5f);
        topPlayerStart.gameObject.SetActive(false);
        bottomPlayerStart.gameObject.SetActive(false);
    }

    public void ShiftPanel(string panel)
    {
        if (panel.Equals("top"))
        {
            topPanel.Shift();
        }
        else
        {
            bottomPanel.Shift();
        }
    }

    public RectTransform GetBlock(float x, float y, string board)
    {
        RectTransform block;
        if (board.Equals("top"))
        {
            block = topPanel.GetBlock(x, y);
        }
        else
        {
            block = bottomPanel.GetBlock(x, y);
        }
        return block;
    }

    public RectTransform GetBlock(float x, float y)
    {
        RectTransform block;
        if (y > -8)
        {
            block = topPanel.GetBlock(x, 7 + y);
        }
        else
        {
            block = bottomPanel.GetBlock(x, 18 + y);
        }
        return block;
    }
}
