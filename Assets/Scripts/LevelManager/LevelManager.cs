using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public Transform container;

    public List<GameObject> levels;

    public List<LevelPieceBasedSetup> levelPieceBasedSetups;

    [Header("Pieces")]
    //public List<LevelPieceBase> levelPieces;
    //public int piecesNumber = 5;
    public float timeBetweenPieces = .2f;

    [SerializeField] private int _index;
    private GameObject _currentLevel;

    private List<LevelPieceBase> _spawnedPieces = new List<LevelPieceBase>();
    private LevelPieceBasedSetup _currSetup;

    public GameObject startPiece;
    private GameObject _startPieceSpawned;
    public GameObject endLinePiece;
    private GameObject _endLinePieceSpawned;

    [Header("Animation")]
    public float scaleDuration = .2f;
    public float scaletimeBetweenPieces = .1f;
    public Ease ease = Ease.OutBack;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", _index);
            PlayerPrefs.Save();
        }
        else
        {
            _index = PlayerPrefs.GetInt("Level");

            if (_index >= levelPieceBasedSetups.Count - 1)
            {
                _index = 0;
            }
            else
            {
                _index++;
            }

            PlayerPrefs.SetInt("Level", _index);
            PlayerPrefs.Save();
        }

        CreateLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            CreateLevel();
        }
    }

    private void SpawnNextLevel()
    {
        if (_currentLevel != null)
        {
            Destroy(_currentLevel);
            _index++;

            if (_index >= levels.Count)
            {
                ResetLevelIndex();
            }
        }

        _currentLevel = Instantiate(levels[_index], container);
        _currentLevel.transform.localPosition = Vector3.zero;
    }

    private void ResetLevelIndex()
    {
        _index = 0;
    }
    #region

    private void CreateLevel()
    {
        CleanSpawnedObject();

        if (_currSetup != null)
        {
            _index++;

            if (_index >= levelPieceBasedSetups.Count)
            {
                ResetLevelIndex();
            }
        }

        _currSetup = levelPieceBasedSetups[_index];

        for (int i = 0; i < _currSetup.piecesNumber; i++)
        {
            CreateLevelPiece();
        }

        var spawnedStartPiece = Instantiate(startPiece, container);
        spawnedStartPiece.transform.position = Vector3.zero;
        _startPieceSpawned = spawnedStartPiece;

        var spawnedEndLinePiece = Instantiate(endLinePiece, container);
        spawnedEndLinePiece.transform.position += new Vector3(0, 0, 10 * _currSetup.piecesNumber);
        _endLinePieceSpawned = spawnedEndLinePiece;

        ColorManager.Instance.ChangeColorByType(_currSetup.artType);

        StartCoroutine(ScalePiecesByTime());
    }

    private void CreateLevelPiece()
    {
        var piece = _currSetup.levelPieces[Random.Range(0, _currSetup.levelPieces.Count)];
        var spawnedPiece = Instantiate(piece, container);

        if (_spawnedPieces.Count > 0)
        {
            var lastPiece = _spawnedPieces[_spawnedPieces.Count - 1];
            spawnedPiece.transform.position = lastPiece.endPiece.position + new Vector3(0,0,5);
        }

        foreach (var p in spawnedPiece.GetComponentsInChildren<ArtPiece>())
        {
            p.ChangePiece(ArtManager.Instance.GetSetupByType(_currSetup.artType).gameObject);
        }

        _spawnedPieces.Add(spawnedPiece);
    }

    IEnumerator ScalePiecesByTime()
    {
        _startPieceSpawned.transform.localScale = new Vector3(.1f, .1f, .1f);
        _endLinePieceSpawned.transform.localScale = new Vector3(.1f, .1f, .1f);
        foreach (var p in _spawnedPieces)
        {
            p.transform.localScale = new Vector3(.1f, .1f, .1f);
        }

        yield return null;

        _startPieceSpawned.transform.DOScale(1, scaleDuration);
        yield return new WaitForSeconds(scaletimeBetweenPieces);

        for (int i = 0; i < _spawnedPieces.Count; i++)
        {
            _spawnedPieces[i].transform.DOScale(1, scaleDuration).SetEase(ease);
            yield return new WaitForSeconds(scaletimeBetweenPieces);
        }

        _endLinePieceSpawned.transform.DOScale(1, scaleDuration);

        CoinsAnimatorManager.Instance.StartAnimations();
    }

    private void CleanSpawnedObject()
    {
        for (int i = _spawnedPieces.Count - 1; i >= 0; i--)
        {
            Destroy ( _spawnedPieces[i].gameObject );
        }

        _spawnedPieces.Clear();
        Destroy(_startPieceSpawned);
        Destroy(_endLinePieceSpawned);
    }

    IEnumerator CreateLevelCoroutine()
    {
        for (int i = 0; i < _currSetup.piecesNumber; i++)
        {
            CreateLevelPiece();
            yield return new WaitForSeconds(timeBetweenPieces);
        }
    }

    #endregion

}
