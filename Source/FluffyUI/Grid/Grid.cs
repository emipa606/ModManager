using System;
using System.Collections.Generic;
using System.Linq;
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
        if ((first == Direction.Up || first == Direction.Down) &&
            (second == Direction.Up || second == Direction.Down) ||
            (first == Direction.Left || first == Direction.Right) &&
            (second == Direction.Left || second == Direction.Right))
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

    public Direction Vertical
    {
        get
        {
            if (firstDirection == Direction.Up || firstDirection == Direction.Down)
            {
                return firstDirection;
            }

            if (secondDirection == Direction.Up || secondDirection == Direction.Down)
            {
                return secondDirection;
            }

            return Direction.Down;
        }
    }

    public bool Up => Vertical == Direction.Up;
    public bool Down => Vertical == Direction.Down;
    public bool Right => Horizontal == Direction.Right;
    public bool Left => Horizontal == Direction.Left;

    public Direction Horizontal
    {
        get
        {
            if (firstDirection == Direction.Left || firstDirection == Direction.Right)
            {
                return firstDirection;
            }

            if (secondDirection == Direction.Left || secondDirection == Direction.Right)
            {
                return secondDirection;
            }

            return Direction.Right;
        }
    }

    public Vector2 Size => rect.size - gutters;

    public Vector2 Available =>
        new Vector2(
            Right ? rect.xMax - (gutters.x / 2f) - pos.x : pos.x - (rect.xMin + (gutters.x / 2f)),
            Down ? rect.yMax - (gutters.y / 2f) - pos.y : pos.y - (rect.yMin + (gutters.y / 2f)));

    public Rect Rect => rect.ContractedBy(gutters / 2);

    public Grid Row(int i = 1)
    {
        //if ( rows > this.rows )
        //    throw new ArgumentOutOfRangeException( nameof( rows ) );
        return Row((float)i / rows);
    }

    public List<Grid> Rows(params int[] ints)
    {
        var sum = ints.Sum();
        return ints.Select(row => Row((float)row / sum)).ToList();
    }

    public List<Grid> Rows(params float[] floats)
    {
        return floats.Select(Row).ToList();
    }

    public Grid Row(float height = 1f)
    {
        height = height > 1f ? height : Size.y * height;
        //if ( height > Available.y + EPSILON )
        //    throw new ArgumentOutOfRangeException( nameof( height ), $"requested: {height}, size: {Size}, available: {Available}" );

        if (Down)
        {
            var row = New(pos.x, pos.y, Available.x, height, gutterAmount: new Vector2(0f, defaultGutters.y));
            pos.y += height;
            return row;
        }
        else
        {
            var row = New(pos.x, pos.y - height, Available.x, height, gutterAmount: new Vector2(0f, defaultGutters.y));
            pos.y -= height;
            return row;
        }
    }

    public Grid Column(int i = 1)
    {
        //if ( cols > this.cols )
        //    throw new ArgumentOutOfRangeException( nameof(cols));
        return Column((float)i / cols);
    }

    public List<Grid> Columns(params int[] ints)
    {
        var sum = ints.Sum();
        return ints.Select(col => Column((float)col / sum)).ToList();
    }

    public List<Grid> Columns(params float[] floats)
    {
        return floats.Select(Column).ToList();
    }

    public Grid Column(float width = 1f)
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

    public Grid New(float x, float y, float width, float height, int? columns = null, int? rowAmount = null,
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