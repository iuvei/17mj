using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TetrisGrid : MonoBehaviour {
    public static int w = 10;
    public static int h = 21;
    public static TetrisGrid Instance;
    public static Transform[,] grid = new Transform[w, h];
    private List<GameObject> readyDelgrid = new List<GameObject>();

    void Start() {
        Instance = this;
    }

    //四捨五入
    public static Vector2 RoundVec2(Vector2 v) {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    //判斷在邊界內
    public static bool InsideBorder(Vector2 v) {
        return ((int) v.x >=0 && (int)v.x < w && (int)v.y >= 0);
    }

    //當 y 列被堆滿，則刪除該列方塊
    public void DeleteRow(int y) {
        MiniGames.Instance.PlayDelSound();
        for (int x = 0; x < w; ++x) {
            grid[x, y].DOBlendableMoveBy(new Vector3(0, 1f, 0), 0.15f).SetEase(Ease.OutSine).SetDelay(0.01f*x);
            grid[x, y].GetComponent<SpriteRenderer>().DOFade(0, 0.15f).SetDelay(0.01f * x);

            readyDelgrid.Add(grid[x, y].gameObject);
            //Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }

        Invoke("DeleteBlock", 0.25f);
        //StartCoroutine(DeleteBlock(0.25f));
        TetrisScore.Instance.WriteScore(20 * (w - 1));
    }

    //當刪除 y-1 列，若 y 列有方塊 則往下降
    public static void DecreaseRow(int y) {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] != null) {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1 , 0);
            }
        }
    }

    // 把第 y 行以上的 都檢查一遍，是否需要下移
    public static void DecreaseRowsAbove(int y) {
        for (int i = y; i < h; ++i)
        {
            DecreaseRow(i);
        }
    }

    //判斷某列是否填滿
    public static bool IsRowFull(int y) {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] == null)
                return false;
        }

        return true;
    }

    //判斷某列是否全空
    public static bool IsRowEmpty()
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, 0] != null)
                return false;

            if (grid[x, 1] != null)
                return false;
        }
        return true;
    }

    // 刪除某列
    public void DeleteFullRows() {
        for (int y = 0; y < h; ++y)
        {
            if (IsRowFull(y)) {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }
        if (IsRowEmpty()) {
            TetrisScore.Instance.ShowExcellent();
            Debug.Log("全消!");
        }
    }

    public void RemoveGrid() {
        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y) {
                if (grid[x, y] != null) {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }

        readyDelgrid.Clear();
    }

    //IEnumerator DeleteBlock(float _delay) {
    //    yield return new WaitForSeconds(_delay);
    //    for (int x = 0; x < w; ++x)
    //    {
    //        Destroy(readyDelgrid[x].gameObject);
    //    }
    //    readyDelgrid.RemoveRange(0, w);
    //}

    private void DeleteBlock()
    {
        for (int x = 0; x < w; ++x)
        {
            Destroy(readyDelgrid[x].gameObject);
        }
        readyDelgrid.RemoveRange(0, w);
    }
}
