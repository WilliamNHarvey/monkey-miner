using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public RectTransform topPanelParent;
    public RectTransform bottomPanelParent;

    private int screens;
    public int colsPerScreen = 32; //TODO: Figure this out
    public int rowsPerPanel = 7;

    private List<Vector2> emptyPositions = new List<Vector2>();
    private Dictionary<Vector2, RectTransform> blockGrid = new Dictionary<Vector2, RectTransform>();

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
    public RectTransform[] wallSwitch;
    public Count wallSwitchCount = new Count(5, 10);
    public RectTransform[] crate;
    public Count crateCount = new Count(10, 20);

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

        public RectTransform GetBlockInSet(float x, float y, Dictionary<Vector2, RectTransform> set)
        {
            foreach (Vector2 loc in set.Keys)
            {
                if (loc.x == x && loc.y == y)
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

        public void PlaceWall(Vector2 loc, int thickness, RectTransform block)
        {
            int left = thickness / 2;
            int right = (int)Math.Ceiling(thickness / 2.0f);
            Vector2 playerCoords = this.player.MonkeyToLucas(this.player.rt.anchoredPosition);
            if (thickness % 2 == 0)
            {
                right--;
            }
            for (int i = -left; i < right; i++)
            {
                for(int j = 0; j < 7; j++)
                {
                    Vector2 curPos = new Vector2(loc.x + i, j + 0.5f);
                    if(playerCoords.x == curPos.x && playerCoords.y == curPos.y)
                    {
                        continue;
                    }
                    SetBlockLocation(curPos, block);
                }
            }
        }

        public void RemoveBlock(Vector2 loc)
        {
            blocks.Remove(loc);
        }

        public void SetBlockLocation(Vector2 loc, RectTransform block)
        {
            RectTransform existing = GetBlock(loc.x, loc.y);
            if (existing && existing.gameObject.activeSelf)
            {
                existing.gameObject.SetActive(false);
            }
            blocks.Remove(loc);
            block = Instantiate(block);
            block.transform.SetParent(parent);
            block.anchoredPosition = loc;
            blocks.Add(loc, block);
        }

        public RectTransform[] GetRandomOfType(string tag, int count)
        {
            Dictionary<Vector2, RectTransform> blocksOfType = new Dictionary<Vector2, RectTransform>();
            foreach (Vector2 loc in blocks.Keys)
            {
                if (blocks[loc].tag == tag && loc.x > 0 && loc.x < 32)
                {
                    blocksOfType[loc] = blocks[loc];
                }
            }
            RectTransform[] res = new RectTransform[count];
            List<Vector2> keyList = new List<Vector2>(blocksOfType.Keys);
            Vector2 randomKey = keyList[Random.Range(0, keyList.Count)];
            List<int> randomList = new List<int>();
            while(randomList.Count < count)
            {
                int rand = Random.Range(0, keyList.Count);
                if (!randomList.Contains(rand))
                    randomList.Add(rand);
            }
            for(int i = 0; i < count; i++)
            {
                res[i] = blocksOfType[keyList[randomList[i]]];
            }
            return res;
        }

        public RectTransform GetRandomBlockOnScreen()
        {
            if(blocks.Keys.Count > 0)
            {
                int x = Random.Range(0, 32);
                int y = Random.Range(0, 7);
                RectTransform block = GetBlock(x + 0.5f, y + 0.5f);
                while (block == null || block.gameObject.activeSelf == false || block.tag == "Goal")
                {
                    x = Random.Range(0, 32);
                    y = Random.Range(0, 7);
                    block = GetBlock(x + 0.5f, y + 0.5f);
                }
                return block;
            }
            return null;
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
                if (x % 32 != 0)
                {
                    emptyPositions.Add(new Vector2(x, y));
                }
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
        for (float x = left - 1; x < left + 6; x++)
        {
            for (float y = 0; y < rowsPerPanel; y++)
            {
                Vector2 pos = new Vector2(x, y);
                emptyPositions.Remove(pos);
                if ((y == bottom || y == bottom + 4) && left <= x && x <= left + 4 || x == left && bottom < y && y < top)
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
        LayoutBlockAtRandom(rockBlocks, rockCount.min, rockCount.max);
        LayoutBlockAtRandom(brickBlocks, brickCount.min, brickCount.max);
        LayoutBlockAtRandom(ironBlocks, ironCount.min, ironCount.max);
        LayoutBlockAtRandom(oil, oilCount.min, oilCount.max);
        LayoutBlockAtRandom(tnt, tntCount.min, tntCount.max);
        LayoutBlockAtRandom(wallSwitch, wallSwitchCount.min, wallSwitchCount.max);
        LayoutBlockAtRandom(crate, crateCount.min, crateCount.max);
        FillWithDirt();
        topPanel.SetupBlocks(blockGrid);
        bottomPanel.SetupBlocks(blockGrid);
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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
