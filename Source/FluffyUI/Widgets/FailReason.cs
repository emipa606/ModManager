// FailReason.cs
// Copyright Karel Kroeze, 2018-2018

namespace FluffyUI;

public struct FailReason
{
    public readonly string Reason;
    private readonly bool Success;

    private FailReason(string reason)
    {
        Success = false;
        Reason = reason;
    }

    private FailReason(bool success)
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