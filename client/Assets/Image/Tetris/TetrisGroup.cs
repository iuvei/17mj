using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisGroup : MonoBehaviour {
    public int _rotateType = 4;
    public Vector2 _nextPivort;
    private float _lastFall = 0;
    private Button[] _btns;
    private Transform _center;
    private bool _fastFall = false;
    private bool _rotateDir = true;
    private int _childNum = 0;


    void Start() {
        _center = transform.Find("center");
        _childNum = _center.childCount;
        _btns = TetrisSpawner.Instance._ctrlBtns;

        if (!IsValidGridPos())
        {
            //transform.position += new Vector3(0, 1, 0);
            TetrisScore.Instance.ShowGameOver(true);
            MiniGames.Instance.PlayOverSound();   

            UnBindBtn(_btns);
            //Destroy(gameObject);
            enabled = false;
        }
        else {
            BindBtn(_btns);
        }
    }


    //判斷每一塊積木內的 小方塊的位置是否合理
    bool IsValidGridPos() {
        foreach (Transform child in _center) {
            Vector2 v = TetrisGrid.RoundVec2(child.position);

            if(!TetrisGrid.InsideBorder(v))
                return false;

            if (TetrisGrid.grid[(int)v.x, (int)v.y] != null && TetrisGrid.grid[(int)v.x, (int)v.y].parent.parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid() {
        for (int y = 0; y < TetrisGrid.h; ++y)
            for (int x = 0; x < TetrisGrid.w; ++x)
                if(TetrisGrid.grid[x, y] != null)
                    if (TetrisGrid.grid[x, y].parent.parent == transform)
                        TetrisGrid.grid[x, y] = null;

        foreach (Transform child in _center)
        {
            Vector2 v = TetrisGrid.RoundVec2(child.position);
            TetrisGrid.grid[(int)v.x, (int)v.y] = child;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0);

            if (IsValidGridPos()) {
                UpdateGrid();
                MiniGames.Instance.PlayMoveSound();
            }
            else
                transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0);

            if (IsValidGridPos()) {
                UpdateGrid();
                MiniGames.Instance.PlayMoveSound();
            }
            else
                transform.position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            switch (_rotateType)
            {
                case 0:
                    break;
                case 2:
                    transform.Rotate(0, 0, (_rotateDir ? 90: -90));
                    if (IsValidGridPos()) {
                        UpdateGrid();
                        MiniGames.Instance.PlayMoveSound();
                        _rotateDir = !_rotateDir;
                    }
                    else
                        transform.Rotate(0, 0, (_rotateDir ? -90 : 90));
                    
                    break;
                case 4:
                default:
                    transform.Rotate(0, 0, -90);
                    if (IsValidGridPos())
                    {
                        UpdateGrid();
                        MiniGames.Instance.PlayMoveSound();
                    }
                        
                    else
                        transform.Rotate(0, 0, 90);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) && !_fastFall)
        {
            _fastFall = true;
            int _y = (int)transform.position.y;
            for (int i = 0; i < _y + 1; i++)
            {
                FindObjectOfType<TetrisScore>().WriteScore(1);
                transform.position += new Vector3(0, -1, 0);
                if (IsValidGridPos()) {
                    UpdateGrid();
                    MiniGames.Instance.PlayFallSound();
                }
                else
                {
                    UnBindBtn(_btns);
                    transform.position += new Vector3(0, 1, 0);

                    TetrisGrid.Instance.DeleteFullRows();
                    FindObjectOfType<TetrisSpawner>().SpawnToMain(0);
                    FindObjectOfType<TetrisScore>().WriteScore(_childNum*10);

                    enabled = false;
                    return;
                }
            }
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Time.time - _lastFall >= 1 )&& !_fastFall)
        //else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !_fastFall)
        {
            FindObjectOfType<TetrisScore>().WriteScore(1);
            transform.position += new Vector3(0, -1, 0);

            if (IsValidGridPos())
                UpdateGrid();
            else
            {
                UnBindBtn(_btns);
                transform.position += new Vector3(0, 1, 0);

                TetrisGrid.Instance.DeleteFullRows();

                FindObjectOfType<TetrisSpawner>().SpawnToMain(0);
                FindObjectOfType<TetrisScore>().WriteScore(_childNum*10);

                enabled = false;
            }

            _lastFall = Time.time;
        }
    }


    public void BindBtn(Button[] btns)
    {
        _btns[0].onClick.AddListener(delegate { ClickLeft(); });
        _btns[1].onClick.AddListener(delegate { ClickRight(); });
        _btns[2].onClick.AddListener(delegate { ClickUp(); });
        _btns[3].onClick.AddListener(delegate { ClickDown(); });
    }

    public void UnBindBtn(Button[] btns)
    {
        _btns[0].onClick.RemoveAllListeners();
        _btns[1].onClick.RemoveAllListeners();
        _btns[2].onClick.RemoveAllListeners();
        _btns[3].onClick.RemoveAllListeners();
    }

    private void ClickLeft()
    {
        transform.position += new Vector3(-1, 0, 0);

        if (IsValidGridPos())
            UpdateGrid();
        else
            transform.position += new Vector3(1, 0, 0);
    }

    private void ClickRight()
    {
        transform.position += new Vector3(1, 0, 0);

        if (IsValidGridPos())
            UpdateGrid();
        else
            transform.position += new Vector3(-1, 0, 0);
    }

    private void ClickUp()
    {
        switch (_rotateType)
        {
            case 0:
                break;
            case 2:
                transform.Rotate(0, 0, (_rotateDir ? 90 : -90));
                if (IsValidGridPos())
                {
                    UpdateGrid();
                    _rotateDir = !_rotateDir;
                }
                else
                    transform.Rotate(0, 0, (_rotateDir ? -90 : 90));

                break;
            case 4:
            default:
                transform.Rotate(0, 0, -90);
                if (IsValidGridPos())
                    UpdateGrid();
                else
                    transform.Rotate(0, 0, 90);
                break;
        }
    }

    private void ClickDown()
    {
        FindObjectOfType<TetrisScore>().WriteScore(1);
        transform.position += new Vector3(0, -1, 0);

        if (IsValidGridPos())
            UpdateGrid();
        else
        {
            UnBindBtn(_btns);
            transform.position += new Vector3(0, 1, 0);

            TetrisGrid.Instance.DeleteFullRows();

            FindObjectOfType<TetrisSpawner>().SpawnToMain(0);

            FindObjectOfType<TetrisScore>().WriteScore(_childNum*10);

            enabled = false;
        }

        _lastFall = Time.time;
    }


    //private void PlayMoveSound() {
    //    //AudioManager.Instance.PlaySE("TetrisMove");
    //    SoundEffect.Instance.PlayMove();
    //}

    //private void PlayOverSound()
    //{
    //    //AudioManager.Instance.PlaySE("TetrisOver");
    //    SoundEffect.Instance.PlayOver();
    //}

    //private void PlayFallSound()
    //{
    //    //AudioManager.Instance.PlaySE("TetrisFall");
    //    SoundEffect.Instance.PlayFall();
    //}


}
