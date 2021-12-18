using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public float swipeAngle = 0f;

    private Board board;
    private GameObject otherPiece;
    private Vector2 firstTouchPos;
    private Vector2 finTouchPos;
    private Vector2 tempPos;
    public bool isMatched = false;
    public float swipeResist = 1f;
    

    private void Start() {
        board = FindObjectOfType<Board>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //previousRow = row;
        //previousColumn = column;
    }

    private void Update() {
        FindMatches();
        if (isMatched) {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, 0.5f);
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1) {
            //Move towards target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.01f);
            if (board.allPieces[column, row] != this.gameObject) {
                board.allPieces[column, row] = this.gameObject;
            }
        }else {
            //Set position directly
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1) {
            //Move towards target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, 0.01f);
            if (board.allPieces[column, row] != this.gameObject) {
                board.allPieces[column, row] = this.gameObject;
            }
        }else {
            //Set position directly
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    public IEnumerator CheckMoveCo() {
        yield return new WaitForSeconds(0.4f);
        if (otherPiece != null) {
            if (!isMatched && !otherPiece.GetComponent<Piece>().isMatched) {
                otherPiece.GetComponent<Piece>().row = row;
                otherPiece.GetComponent<Piece>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.3f);
                board.currentState = GameState.move;
            }else{
                board.DestroyMatches();
            }
            otherPiece = null;
        }
    }
    
    private void OnMouseDown() {
        if(board.currentState == GameState.move)
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp() {
        if (board.currentState == GameState.move) {
            finTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalcAngle();
        }
    }

    void CalcAngle() {
        if (Mathf.Abs(finTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finTouchPos.x - firstTouchPos.x) > swipeResist) {
            swipeAngle = Mathf.Atan2(finTouchPos.y - firstTouchPos.y, finTouchPos.x - firstTouchPos.x) * 180/Mathf.PI;
            MovePieces();
            board.currentState = GameState.wait;
        }else {
            board.currentState = GameState.move;
        }
    }

    void MovePieces() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            //Right swipe
            otherPiece = board.allPieces[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherPiece.GetComponent<Piece>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            //Up swipe
            otherPiece = board.allPieces[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherPiece.GetComponent<Piece>().row -= 1;
            row += 1;
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            //Left swipe
            otherPiece = board.allPieces[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherPiece.GetComponent<Piece>().column += 1;
            column -= 1;
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            //Down swipe
            otherPiece = board.allPieces[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherPiece.GetComponent<Piece>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMoveCo());
    }

    void FindMatches() {
        if (column > 0 && column < board.width - 1) {
            GameObject leftPiece1 = board.allPieces[column - 1, row];
            GameObject rightPiece1 = board.allPieces[column + 1, row];
            if (leftPiece1 != null && rightPiece1 != null && leftPiece1 != this.gameObject &&
                rightPiece1 != this.gameObject) {
                if (leftPiece1.tag == this.gameObject.tag && rightPiece1.tag == this.gameObject.tag) {
                    leftPiece1.GetComponent<Piece>().isMatched = true;
                    rightPiece1.GetComponent<Piece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1) {
            GameObject upPiece1 = board.allPieces[column, row + 1];
            GameObject downPiece1 = board.allPieces[column, row - 1];
            if (upPiece1 != null && downPiece1 != null && upPiece1 != this.gameObject &&
                downPiece1 != this.gameObject) {
                if (upPiece1.tag == this.gameObject.tag && downPiece1.tag == this.gameObject.tag) {
                    upPiece1.GetComponent<Piece>().isMatched = true;
                    downPiece1.GetComponent<Piece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
