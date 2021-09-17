using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Puzzle : MonoBehaviour
{
    #region fields
    #region serialize fields
    [Header("Shuffle")]
    [SerializeField]
    private int shuffleLength;

    [Header("Board & move")]
    [SerializeField] private float defaultMoveDuration = 0.3f;
    [SerializeField] private float shuffleMoveMoveDuration = 0.1f;
    [UnityEngine.Range(1,10)]
    [SerializeField] private int blocksPerLine = 4;
    [SerializeField] private Texture2D[] Animalsimages;
    [SerializeField] private Texture2D[] Techimages;
    [SerializeField] private Texture2D[] Archimages;
    [SerializeField] private Texture2D[] Artsimages;
    [SerializeField] private Text timer;
    [SerializeField] private Text mover;
    #endregion serialize fields

    private int moveCount=0;
    private bool isShuffled = false;
    private bool isPlaying = true;
    private Texture2D image;
    private int imageCount;
    private int complexityCount;
    private string categories;
    private AudioSource sound;
    private Camera _camera;
    private Block _emptyBlock;
    private Block[,] _blocks;
    private Queue<Block> moveQueue = new Queue<Block>();
    private Coroutine _animationCoroutine;
    private int _shuffleMovesRemaining;
    private Vector2Int _previousShuffleOffset;
    #endregion fields
    private enum PuzzleState
    {
        Solved,
        Shuffling,
        InPlay
    };

    private PuzzleState _state;
    private bool _blockIsMoving;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        sound = gameObject.GetComponent<AudioSource>();
        AdjustCameraView(blocksPerLine);
        StartCoroutine(Timer());
        imageCount = PlayerPrefs.GetInt("CurrentImage", 0);
        complexityCount = PlayerPrefs.GetInt("CurrentComplexity", 1);
        categories= PlayerPrefs.GetString("CurrentCategories", "Animals");

        switch (categories)
        {
            case "Animals":
                image = Animalsimages[imageCount - 1];
                break;
            case "Tech":
                image = Techimages[imageCount - 1];
                break;
            case "Architecture":
                image = Archimages[imageCount - 1];
                break;
            case "Arts":
                image = Artsimages[imageCount - 1];
                break;

        }
        blocksPerLine = complexityCount + 2;
        InstantiateQuads(blocksPerLine);
    }
    public void StartGame()
    {
        if (_state == PuzzleState.Solved)
        {
            StartShuffle();
        }
    }
    IEnumerator Timer()
    {
        int sec = 0;
        int min = 0;
        while (isPlaying)
        {
            yield return new WaitForSeconds(1);
            if (sec >= 10)
            {
                timer.text = $"{min}:{sec}";
            }
            else
            {
                timer.text = $"{min}:0{sec}";
            }
            sec++;
            if (sec >= 60)
            {
                sec = 0;
                min++;
            }
        }
    }
    private void StartShuffle()
    {
        _state = PuzzleState.Shuffling;
        _emptyBlock.gameObject.SetActive(false);
        _shuffleMovesRemaining = shuffleLength;

        MakeNextShuffleMove();
    }

    public void InstantiateQuads(int blocksPerLine)
    {
        _blocks = new Block[blocksPerLine, blocksPerLine];
        var imageSlices = ImageSlicer.GetSlices(image, this.blocksPerLine);
        var offset = blocksPerLine / 2f - 0.5f;

        for (int row = 0; row < blocksPerLine; row++)
        {
            for (int column = 0; column < blocksPerLine; column++)
            {
                var instantiatePosition = new Vector3(row - offset, column - offset, 0f);
                var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = instantiatePosition;
                quad.transform.SetParent(transform);
                quad.gameObject.name = $"quad{row}x{column}";
                var block = quad.AddComponent<Block>();
                block.OnBlockPressed += PlayerMoveBlockInput;
                block.OnFinishedMoving += HandleBlockFinishedMoving;

                block.Init(new Vector2Int(row, column), imageSlices[row, column]);
                _blocks[row, column] = block;
            }
        }
        _emptyBlock = _blocks[blocksPerLine - 1, blocksPerLine - 1];
    }

    //TODO: make this adjustment happens when resolution change
    private void AdjustCameraView(int blocksPerLine)
    {
        const float offset = 1f;
        if (_camera == null)
        {
            throw new NullReferenceException();
        }

        if (_camera.aspect > 1)
        {
            _camera.orthographicSize = (blocksPerLine / 2f) + offset;
        }
        else
        {
            _camera.orthographicSize = (blocksPerLine / 2f / _camera.aspect) + offset;
        }
    }

    /// <summary>
    /// Move a block if allowed
    /// </summary>
    private void PlayerMoveBlockInput(Block blockToMove)
    {
        if (_state == PuzzleState.InPlay)
        {
            moveQueue.Enqueue(blockToMove);
            MakeNextPlayerMove();
        }
    }

    private void MoveBlock(Block blockToMove, float duration)
    {
        if (!IsValidMove(blockToMove.coord))
        {
            return;
        }
        if (isShuffled)
        {
            moveCount++;
            sound.pitch = Random.Range(0.9f,1.1f);
            sound.Play();
            mover.text = moveCount.ToString();
        }

        _blocks[blockToMove.coord.x, blockToMove.coord.y] = _emptyBlock;
        _blocks[_emptyBlock.coord.x, _emptyBlock.coord.y] = blockToMove;

        // Change the coord's
        var tempCoord = _emptyBlock.coord;
        _emptyBlock.coord = blockToMove.coord;
        blockToMove.coord = tempCoord;

        // Change the transform's
        var positionToMove = _emptyBlock.transform.position;
        _emptyBlock.transform.position = blockToMove.transform.position;
        _blockIsMoving = true;
        blockToMove.MoveToPosition(positionToMove, duration);
    }

    private void HandleBlockFinishedMoving()
    {
        _blockIsMoving = false;
        if (CheckIfSolved())
        {
            // Wait a few seconds and restart
            StartCoroutine(DisplayStartMessageAfterSeconds(3f));
        }

        if (_state == PuzzleState.InPlay)
        {
            MakeNextPlayerMove();
            isShuffled = true;
        }
        else if (_state == PuzzleState.Shuffling)
        {
            if (_shuffleMovesRemaining > 0)
            {
                MakeNextShuffleMove();
            }
            else
            {
                _state = PuzzleState.InPlay;
            }
        }
    }

    private IEnumerator DisplayStartMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void MakeNextPlayerMove()
    {
        while (moveQueue.Count > 0 && !_blockIsMoving)
        {
            MoveBlock(moveQueue.Dequeue(), defaultMoveDuration);
        }
    }

    private bool IsValidMove(Block blockToMove)
    {
        return IsValidMove(blockToMove.coord);
    }

    private bool IsValidMove(Vector2Int coord)
    {
        return (coord - _emptyBlock.coord).sqrMagnitude == 1;
    }

    private void MakeNextShuffleMove()
    {
        Vector2Int[] offsets =
        {
            new Vector2Int (1, 0),    // right
            new Vector2Int( 0, 1),    // above
            new Vector2Int(-1, 0),    // bellow
            new Vector2Int( 0,-1)     // left
        };

        int randomIndex = Random.Range(0, offsets.Length);
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];
            if (offset != _previousShuffleOffset * -1)
            {
                Vector2Int moveBlockCoord = _emptyBlock.coord + offset;
                if (IsInsideBoundary(moveBlockCoord))
                {
                    MoveBlock(_blocks[moveBlockCoord.x, moveBlockCoord.y], shuffleMoveMoveDuration);
                    _shuffleMovesRemaining--;
                    _previousShuffleOffset = offset;
                    break;
                }
            }
        }
        
    }

    private bool IsInsideBoundary(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < blocksPerLine && coord.y >= 0 && coord.y < blocksPerLine;
    }

    private bool CheckIfSolved()
    {
        foreach (var block in _blocks)
        {
            if (!block.IsAtStartingCoord())
            {
                return false;
            }
        }

        _state = PuzzleState.Solved;
        //TODO: show ads
        int pastComplexity = PlayerPrefs.GetInt($"{imageCount}", 0);
        if (complexityCount > pastComplexity)
        {
            PlayerPrefs.SetInt($"{categories}-{imageCount}", complexityCount);
            PlayerPrefs.SetInt(categories, PlayerPrefs.GetInt(categories, 0) + (complexityCount - pastComplexity));
        }
        isPlaying = false;
        _emptyBlock.gameObject.SetActive(true);
        return true;
    }
}

