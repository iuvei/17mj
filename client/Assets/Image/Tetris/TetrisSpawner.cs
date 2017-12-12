using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrisSpawner : MonoBehaviour {
    public GameObject[] _groups;
    public Transform _next;
    [Header("Control Btns: Left、Right、Up、Down")]
    public Button[] _ctrlBtns;
    public Button _retryBtn;
    public static TetrisSpawner Instance;
    private GameObject nextBlock;

    public void SpawnNext() {
        int i = Random.Range(0, _groups.Length);

        if (_next) {
            //GameObject go = Instantiate(_groups[i], _next.position, Quaternion.identity); //Instantiate(_groups[i]);
            GameObject go = Instantiate(_groups[i]);
            //go.GetComponent<TetrisGroup>().enabled = false;
            go.transform.SetParent(_next);
            go.transform.localPosition = go.GetComponent<TetrisGroup>()._nextPivort;
            go.transform.localScale = Vector3.one;
            go.GetComponent<TetrisGroup>().enabled = false;
            nextBlock = go;
        }
    }

    public void SpawnToMain(float _delay)
    {
        StopAllCoroutines();

        StartCoroutine("Spawn", _delay);
    }

    void Start() {
        Instance = this;

        SpawnToMain(0.1f);
    }

    public void ReTry() {
        TetrisGrid.Instance.RemoveGrid();
        TetrisScore.Instance.ShowGameOver(false);

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        SpawnToMain(1);
    }

    private IEnumerator Spawn(float _delay) {

        yield return new WaitForSeconds(_delay);
        while (nextBlock == null)
        {
            SpawnNext();
            yield return new WaitForSeconds(1f);
        }

        GameObject go = Instantiate(nextBlock, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.GetComponent<TetrisGroup>().enabled = true;
        Destroy(nextBlock);
        SpawnNext();
    }


}
