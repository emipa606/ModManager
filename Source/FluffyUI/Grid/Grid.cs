using System;
using UnityEngine;

namespace FluffyUI;

public class Grid
{
    private readonly int cols;
    private readonly Vector2 defaultGutters;
    private readonly Direction firstDirection;
    private readonly Vector2 gutters;
    private readonly int rows;
    private readonly Direction secondDirection;
    private Vector2 pos;
    private Rect rect;

    public Grid(Rect rect, int cols = 12, int rows = 1, Vector2? gutters = null, Vector2? defaultGutters = null,
        Direction first = Direction.Right, Direction second = Direction.Down)
    {
        if (first is Direction.Up or Direction.Down &&
            second is Direction.Up or Direction.Down ||
            first is Direction.Left or Direction.Right &&
            second is Direction.Left or Direction.Right)
        {
            throw new Exception("first and second direction can no both be horizontal/vertical");
        }

        this.rect = rect;
        this.cols = cols;
        this.rows = rows;
        firstDirection = first;
        secondDirection = second;
        this.gutters = gutters ?? new Vector2(6f, 6f);
        this.defaultGutters = defaultGutters ?? this.gutters;
        pos = new Vector2(
            Right ? rect.xMin + (this.gutters.x / 2f) : rect.xMax - (this.gutters.x / 2f),
            Down ? rect.yMin + (this.gutters.y / 2f) : rect.yMax - (this.gutters.y / 2f));
    }

    private Direction Vertical
    {
        get
        {
            if (firstDirection is Direction.Up or Direction.Down)
            {
                return firstDirection;
            }

            return secondDirection is Direction.Up or Direction.Down ? secondDirection : Direction.Down;
        }
    }

    public bool Up => Vertical == Direction.Up;
    private bool Down => Vertical == Direction.Down;
    private bool Right => Horizontal == Direction.Right;
    public bool Left => Horizontal == Direction.Left;

    private Direction Horizontal
    {
        get
        {
            if (firstDirection is Direction.Left or Direction.Right)
            {
                return firstDirection;
            }

            return secondDirection is Direction.Left or Direction.Right ? secondDirection : Direction.Right;
        }
    }

    private Vector2 Size => rect.size - gutters;

    private Vector2 Available =>
        new Vector2(
            Right ? rect.xMax - (gutters.x / 2f) - pos.x : pos.x - (rect.xMin + (gutters.x / 2f)),
            Down ? rect.yMax - (gutters.y / 2f) - pos.y : pos.y - (rect.yMin + (gutters.y / 2f)));

    public Rect Rect => rect.ContractedBy(gutters / 2);

    public Grid Column(int i = 1)
    {
        //if ( cols > this.cols )
        //    throw new ArgumentOutOfRangeException( nameof(cols));
        return Column((float)i / cols);
    }

    private Grid Column(float width = 1f)
    {
        width = width > 1f ? width : Size.x * width;

        if (Right)
        {
            var col = New(pos.x, pos.y, width, Available.y, gutterAmount: new Vector2(defaultGutters.x, 0f));
            pos.x += width;
            return col;
        }
        else
        {
            var col = New(pos.x - width, pos.y, width, Available.y, gutterAmount: new Vector2(defaultGutters.x, 0f));
            pos.x -= width;
            return col;
        }
    }

    private Grid New(float x, float y, float width, float height, int? columns = null, int? rowAmount = null,
        Vector2? gutterAmount = null, Vector2? defaultGutterValue = null, Direction? first = null,
        Direction? second = null)
    {
        return new Grid(new Rect(x, y, width, height), columns ?? cols, rowAmount ?? rows, gutterAmount ?? gutters,
            defaultGutterValue ?? defaultGutters, first ?? firstDirection, second ?? secondDirection);
    }

    public static implicit operator Rect(Grid grid)
    {
        return grid.Rect;
    }
}