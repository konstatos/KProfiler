using System;

public static class DelegatExt
{
    public static bool IsSubscribed(this Delegate evnt, Delegate subs) =>
        evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method);
    public static bool IsSubscribed(this Delegate evnt, Action subs) =>
        evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method);
    public static bool IsSubscribed<T>(this Delegate evnt, Action<T> subs) =>
        evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method);

    public static void SubscribeOnce<T>(ref Action<T> evnt, Action<T> subs)
    {
        if (evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method))
            CLog.Info("IsSubscribed: " + subs.Method.Name);
        else
        {
            evnt += subs;
            CLog.Info($"Subscribe: {evnt.Method.Name} += {subs.Method.Name} L={evnt.GetInvocationList().Length}");
        }
    }
    public static void SubscribeOnce(ref Action evnt, Action subs)
    {
        if (evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method))
            CLog.Info("IsSubscribed: " + subs.Method.Name);
        else
        {
            evnt += subs;
            CLog.Info($"Subscribe: {evnt.Method.Name} += {subs.Method.Name} L={evnt.GetInvocationList().Length}");
        }
    }
    public static void SubscribeOnce<T1, T2>(ref Action<T1, T2> evnt, Action<T1, T2> subs)
    {
        if (evnt != null && evnt.GetInvocationList().Exists(x => x.Method == subs.Method))
            CLog.Info("IsSubscribed: " + subs.Method.Name);
        else
        {
            evnt += subs;
            CLog.Info($"Subscribe: {evnt.Method.Name} += {subs.Method.Name} L={evnt.GetInvocationList().Length}");
        }
    }

    public static void UnsubscribeForce(ref Action delegRef, Action deleg)
    {
        try
        {
            if (delegRef == null) return;
            var array = delegRef.GetInvocationList();
            for (var i = 0; i < array.Length; i++)
            {
                var item = (Action)array[i];
                if (item.Method == deleg.Method)
                {
                    delegRef -= item;
                }
                CLog.Info($"UnsubscribeForce {i} {item.Method.Name}/{deleg.Method.Name}");
            }
        }
        catch (Exception e)
        {
            CLog.Error("UnsubscribeForce", e);
        }
    }

    public static void UnsubscribeForce<T>(ref Action<T> delegRef, Action<T> deleg)
    {
        try
        {
            if (delegRef == null) return;
            var array = delegRef.GetInvocationList();
            for (var i = 0; i < array.Length; i++)
            {
                var item = (Action<T>)array[i];
                if (item.Method == deleg.Method)
                {
                    delegRef -= item;
                }
                CLog.Info($"UnsubscribeForce {i} {item.Method.Name}/{deleg.Method.Name}");
            }
        }
        catch (Exception e)
        {
            CLog.Error("UnsubscribeForce", e);
        }
    }

    public static void UnsubscribeForce<T1, T2>(ref Action<T1, T2> delegRef, Action<T1, T2> deleg)
    {
        try
        {
            if (delegRef == null) return;
            var array = delegRef.GetInvocationList();
            for (var i = 0; i < array.Length; i++)
            {
                var item = (Action<T1, T2>)array[i];
                if (item.Method == deleg.Method)
                {
                    delegRef -= item;
                }
                CLog.Info($"UnsubscribeForce {i} {item.Method.Name}/{deleg.Method.Name}");
            }
        }
        catch (Exception e)
        {
            CLog.Error("UnsubscribeForce", e);
        }
    }
}
