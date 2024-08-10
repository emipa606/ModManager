// FailReason.cs
// Copyright Karel Kroeze, 2018-2018

namespace FluffyUI;

public struct FailReason
{
    public readonly string Reason;
    public readonly bool Success;

    public FailReason(string reason)
    {
        Success = false;
        Reason = reason;
    }

    public FailReason(bool success)
    {
        Success = success;
        Reason = string.Empty;
    }

    public static implicit operator bool(FailReason reason)
    {
        return reason.Success;
    }

    public static implicit operator FailReason(string reason)
    {
        return new FailReason(reason);
    }

    public static implicit operator FailReason(bool success)
    {
        return new FailReason(success);
    }
}