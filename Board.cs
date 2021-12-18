using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState { wait, move }

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int height; //8
    public int width; //86
    public int offset;
    public GameObject tilePrefab;
    public GameObject[] pieces;
    public GameObject[,] allPieces;
    
    private Tile[,] allTiles;
    
    
    void Start()
    {
        allTiles = new Tile[width, height];
        allPieces = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector2 tempPos = new Vector2(i, j + offset);
                GameObject tile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                tile.transform.parent = this.transform;
                tile.name = "Tile: ( " + i + ", " + j + " )";
                
                int pieceToPut = Random.Range(0, pieces.Length);
                
                int maxIterations = 0;
                while (MatchesAt(i, j, pieces[pieceToPut]) && maxIterations < 100) {
                    pieceToPut = Random.Range(0, pieces.Length);
                    maxIterations++;
                    //Debug.Log(maxIterations);
                }
                maxIterations = 0;
                
                GameObject piece = Instantiate(pieces[pieceToPut], tempPos, Quaternion.identity);
                piece.GetComponent<Piece>().row = j;
                piece.GetComponent<Piece>().column = i;
                
                piece.transform.parent = this.transform;
                piece.name = "Piece: ( " + i + ", " + j + " )";
                allPieces[i, j] = piece;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) {
        if (column > 1 && row > 1) {
            if (allPieces[column - 1, row].tag == piece.tag && allPieces[column - 2, row].tag == piece.tag) {
                return true;
            }
            if (allPieces[column, row - 1].tag == piece.tag && allPieces[column, row - 2].tag == piece.tag) {
                return true;
            }
        }
        else if (column <= 1 || row <= 1) {
            if (row > 1) {
                if (allPieces[column, row - 1].tag == piece.tag && allPieces[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1) {
                if (allPieces[column - 1, row].tag == piece.tag && allPieces[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void DestroyMatchAt(int column, int row) {
        if(allPieces[column, row].GetComponent<Piece>().isMatched) {
            Destroy(allPieces[column, row]);
            allPieces[column, row] = null;
        }
    }

    public void DestroyMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allPieces[i, j] != null) {
                    DestroyMatchAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo() {
        int nullCount = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allPieces[i, j] == null) {
                    nullCount++;
                }else if (nullCount > 0) {
                    allPieces[i, j].GetComponent<Piece>().row -= nullCount;
                    allPieces[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allPieces[i, j] == null) {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    int pieceToUse = Random.Range(0, pieces.Length);
                    GameObject piece = Instantiate(pieces[pieceToUse], tempPos, Quaternion.identity);
                    allPieces[i, j] = piece;
                    piece.GetComponent<Piece>().row = j;
                    piece.GetComponent<Piece>().column = i;
                    piece.transform.parent = this.transform;
                    piece.name =  "( " + i + ", " + j + " )";
                }
            }
        }
    }

    private bool MachesOnBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allPieces[i, j] != null) {
                    if (allPieces[i, j].GetComponent<Piece>().isMatched) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);
        while (MachesOnBoard()) {
            yield return new WaitForSeconds(0.1f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;

    }
    
}
