using System;
using System.Collections.Generic;

/// <summary>
/// Пропускатель кадров - при установке в Update возвращает true с указанным периодом
/// (c) konstatos
/// </summary>
public struct FrameSkiper
{
    public float Interval;
    float _nextTime;

    public FrameSkiper(float secondsInterval)
    {
        Interval = secondsInterval;
        _nextTime = TimeExt.TimeFromStartup + secondsInterval;
    }

    /// <summary> Возвращает true, если истек период Interval </summary> 
    public bool Frame()
    {
        if (_nextTime > TimeExt.TimeFromStartup) return false;
        _nextTime = TimeExt.TimeFromStartup + Interval;
        return true;
    }

    public void SetNextAfter(float seconds) => _nextTime = TimeExt.TimeFromStartup + seconds;
}

/// <summary>
/// За указанный период перебирает указанную коллекцию при установке в апдейт
/// (c) konstatos
/// </summary>
public struct FrameSkiperList
{
    public float Interval;
    float _currentInterval;
    float _nextTime;
    int _current;

    public FrameSkiperList(float secondsInterval)
    {
        Interval = secondsInterval;
        _currentInterval = secondsInterval;
        _nextTime = 0;
        _current = 0;
        //GetList = getList;
    }

    /// <summary> Возвращает true, если истек период Interval </summary> 
    public T Frame<T>(List<T> list, Action<T> doWithItem) where T : class
    {
        if (_nextTime > TimeExt.TimeFromStartup) return null;
        if (list.Count == 0)
        {
            _nextTime = TimeExt.TimeFromStartup + Interval;
            return null;
        }
        _currentInterval = Interval / list.Count;
        _nextTime = TimeExt.TimeFromStartup + _currentInterval;
        if (_current >= list.Count) _current = 0;
        var b = list[_current];
        ++_current;
        doWithItem(b);
        return b;
    }
}