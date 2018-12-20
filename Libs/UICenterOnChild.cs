using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(ScrollRect))]
public class UICenterOnChild : MonoBehaviour, IEndDragHandler
{
    public delegate void OnCenterCallback(int page);
    public OnCenterCallback onCenter;

    public Transform Content;
    public float snapWatchOffset = 0.5f;
    public float snapTweenTime = 0.2f;

    private ScrollRect _scrollRect;
    private float _perstep;
    private bool _snapInertia;
    private int _snapCellViewIndex;

    public int CellNum
    {
        get { return _cellNum; }
    }
    private int _cellNum;

    public int CurCellIndex
    {
        get { return _curCellIndex; }
    }

    private int _curCellIndex = 0;

    public float ScrollPosition
    {
        get
        {
            return _scrollRect.horizontal ? _scrollRect.horizontalNormalizedPosition : _scrollRect.verticalNormalizedPosition;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 1);
            if (_scrollRect.horizontal)
            {
                _scrollRect.horizontalNormalizedPosition = value;
            }
            else
            {
                _scrollRect.verticalNormalizedPosition = value;
            }
        }
    }

    // Use this for initialization
    void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    void OnValidate()
    {
        snapWatchOffset = Mathf.Clamp01(snapWatchOffset);
    }

    void OnEnable()
    {
        ResetItem();
        Recenter();
    }

    void OnDisable()
    {

    }

    public void ResetItem()
    {
        _cellNum = 0;
        foreach (Transform item in Content)
        {
            if (item.gameObject.activeSelf)
            {
                ++_cellNum;
            }
        }

        _perstep = 1f / (_cellNum - 1);
    }

    public int FindCellIndexAtNormalizedPos(float normalizedPos)
    {
        // call the overrloaded method on the entire range of the list
        return _FindCellIndexAtNormalizedPos(normalizedPos, 0, _cellNum - 1);
    }

    private int _FindCellIndexAtNormalizedPos(float normalizedPos, int startIndex, int endIndex)
    {
        // if the range is invalid, then we found our index, return the start index
        if (startIndex >= endIndex) return startIndex;

        // determine the middle point of our binary search
        var midIndex = (startIndex + endIndex) / 2;

        // if the middle index is greater than the position, then search the last
        if (GetCellScrollPos(midIndex) + _perstep * snapWatchOffset >= normalizedPos)
            return _FindCellIndexAtNormalizedPos(normalizedPos, startIndex, midIndex);
        else
            return _FindCellIndexAtNormalizedPos(normalizedPos, midIndex + 1, endIndex);
    }

    public float GetCellScrollPos(int cellIndex)
    {
        return _perstep * cellIndex;
    }

    [ContextMenu("MovePrevious")]
    public void MovePrevious()
    {
        JumpTo(_curCellIndex - 1, snapTweenTime, SnapJumpComplete);
    }

    [ContextMenu("MoveNext")]
    public void MoveNext()
    {
        JumpTo(_curCellIndex + 1, snapTweenTime, SnapJumpComplete);
    }

    [ContextMenu("Recenter")]
    public void Recenter()
    {
        if (_cellNum == 0) return;

        // stop the scroller
        _scrollRect.velocity = Vector2.zero;

        // cache the current inertia state and turn off inertia
        _snapInertia = _scrollRect.inertia;
        _scrollRect.inertia = false;

        _snapCellViewIndex = FindCellIndexAtNormalizedPos(ScrollPosition);

        JumpTo(_snapCellViewIndex, snapTweenTime, SnapJumpComplete);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Recenter();
    }

    public void JumpTo(int cellIndex, float tweenTime = 0f, TweenCallback jumpComplete = null)
    {
        if (cellIndex < 0 || cellIndex >= _cellNum)
            return;

        _curCellIndex = cellIndex;

        if (_scrollRect.horizontal)
            _scrollRect.DOHorizontalNormalizedPos(GetCellScrollPos(cellIndex), tweenTime).OnComplete(jumpComplete);
        else
            _scrollRect.DOVerticalNormalizedPos(GetCellScrollPos(cellIndex), tweenTime).OnComplete(jumpComplete);
    }


    private void SnapJumpComplete()
    {
        _scrollRect.inertia = _snapInertia;

        if (onCenter != null) onCenter(_snapCellViewIndex);
    }
}