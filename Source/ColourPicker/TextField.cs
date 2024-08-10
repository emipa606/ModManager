// TextField.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using UnityEngine;
using Verse;

namespace ColourPicker;

public class TextField<T>(
    T value,
    string id,
    Action<T> callback,
    Func<string, T> parser = null,
    Func<string, bool> validator = null,
    Func<T, string> toString = null)
{
    private string _temp = value.ToString();
    private T _value = value;

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            _temp = toString?.Invoke(value) ?? value.ToString();
        }
    }

    public static TextField<float> Float01(float value, string id, Action<float> callback)
    {
        return new TextField<float>(value, id, callback, float.Parse, Validate01, f => Round(f).ToString());
    }

    public static TextField<string> Hex(string value, string id, Action<string> callback)
    {
        return new TextField<string>(value, id, callback, hex => hex, ValidateHex);
    }

    public void Draw(Rect rect)
    {
        var valid = validator?.Invoke(_temp) ?? true;
        GUI.color = valid ? Color.white : Color.red;
        GUI.SetNextControlName(id);
        var temp = Widgets.TextField(rect, _temp);
        GUI.color = Color.white;

        if (temp == _temp)
        {
            return;
        }

        _temp = temp;
        if (!(validator?.Invoke(_temp) ?? true))
        {
            return;
        }

        _value = parser(_temp);
        callback?.Invoke(_value);
    }

    private static bool Validate01(string value)
    {
        if (!float.TryParse(value, out var parsed))
        {
            return false;
        }

        return parsed is >= 0f and <= 1f;
    }

    private static bool ValidateHex(string value)
    {
        return ColorUtility.TryParseHtmlString(value, out _);
    }

    private static float Round(float value, int digits = 2)
    {
        var exponent = Mathf.Pow(10, digits);
        return Mathf.RoundToInt(value * exponent) / exponent;
    }
}