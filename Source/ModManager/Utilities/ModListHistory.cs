// ModListHistory.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Linq;

namespace ModManager;

/// <summary>
///     Manages undo/redo history for the active mod list.
///     Stores snapshots of mod order and allows navigation through history.
/// </summary>
public class ModListHistory
{
    private readonly Stack<ModListSnapshot> _redoStack = new();
    private readonly Stack<ModListSnapshot> _undoStack = new();

    /// <summary>
    ///     Gets whether an undo operation is possible.
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    ///     Gets whether a redo operation is possible.
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    ///     Gets the number of undo operations available.
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    ///     Gets the number of redo operations available.
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    ///     Records the current state of the mod list.
    ///     Clears the redo stack as new changes invalidate future history.
    /// </summary>
    /// <param name="modOrder">List of mod identifiers in their current load order.</param>
    public void RecordState(List<string> modOrder)
    {
        if (modOrder == null || modOrder.Count == 0)
        {
            return;
        }

        // Create snapshot
        var snapshot = new ModListSnapshot(modOrder);

        // Don't record duplicate states
        if (_undoStack.Count > 0 && _undoStack.Peek().Equals(snapshot))
        {
            return;
        }

        _undoStack.Push(snapshot);
        _redoStack.Clear(); // Clear redo history when new change is made
    }

    /// <summary>
    ///     Performs an undo operation, returning the previous mod list state.
    /// </summary>
    /// <param name="currentState">The current state before undoing (to be saved for redo).</param>
    /// <returns>List of mod identifiers in the previous order, or null if undo is not possible.</returns>
    public List<string> Undo(List<string> currentState)
    {
        if (!CanUndo)
        {
            return null;
        }

        // Save the current state to redo stack so we can redo back to it
        if (currentState is { Count: > 0 })
        {
            _redoStack.Push(new ModListSnapshot(currentState));
        }

        // Get and return the previous state
        var previousState = _undoStack.Pop();
        return [..previousState.ModOrder];
    }

    /// <summary>
    ///     Performs a redo operation, returning the next mod list state.
    /// </summary>
    /// <param name="currentState">The current state before redoing (to be saved for undo).</param>
    /// <returns>List of mod identifiers in the next order, or null if redo is not possible.</returns>
    public List<string> Redo(List<string> currentState)
    {
        if (!CanRedo)
        {
            return null;
        }

        // Save the current state to undo stack so we can undo back to it
        if (currentState is { Count: > 0 })
        {
            _undoStack.Push(new ModListSnapshot(currentState));
        }

        // Get and return the next state
        var nextState = _redoStack.Pop();
        return [..nextState.ModOrder];
    }

    /// <summary>
    ///     Clears all undo/redo history.
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    /// <summary>
    ///     Represents a snapshot of the mod list at a point in time.
    /// </summary>
    private class ModListSnapshot(List<string> modOrder) : IEquatable<ModListSnapshot>
    {
        public List<string> ModOrder { get; } = [..modOrder];
        public DateTime Timestamp { get; } = DateTime.Now;

        public bool Equals(ModListSnapshot other)
        {
            if (other == null || ModOrder.Count != other.ModOrder.Count)
            {
                return false;
            }

            return ModOrder.SequenceEqual(other.ModOrder);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ModListSnapshot);
        }

        public override int GetHashCode()
        {
            // Simple hash code based on count and first/last elements
            if (ModOrder == null || ModOrder.Count == 0)
            {
                return 0;
            }

            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + ModOrder.Count;
                hash = (hash * 23) + (ModOrder[0]?.GetHashCode() ?? 0);
                if (ModOrder.Count > 1)
                {
                    hash = (hash * 23) + (ModOrder[^1]?.GetHashCode() ?? 0);
                }

                return hash;
            }
        }
    }
}