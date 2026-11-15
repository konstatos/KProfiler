using System;
using System.Collections.Generic;

public class DoList
{
    public enum EPriority { No, Idle, Low, Normal, High, Immidiate }
    [Flags]
    public enum EPlace
    {
        No,
        In3d = 2,
        /// <summary> В интерфейсах за исключение диалогов </summary>
        InUI = 4,
        InDialog = 8,
        InHyper = 16,
        InUIOr3d = In3d | InUI,
        Any = InUI | In3d | InDialog | InHyper,
    }

    public class DoItem
    {
        public Action Action;
        public EPriority Priority;
        public EPlace Place;

        public override string ToString() => $"Do: {Priority} {Place} {Action}";
    }

    public static DoList I = new DoList();
    public List<DoItem> Items = new List<DoItem>();

    public void Show(Action action, EPlace place, EPriority priority = EPriority.Normal)
    {
        var it = new DoItem() { Action = action, Priority = priority, Place = place };
        CLog.Info("AddTodo: " + it);
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Priority < priority)
            {
                Items.Insert(i, it);
                return;
            }
        }
        Items.Add(it);
    }

    public DoItem? TryDo(bool ui, bool dialog, bool hyper)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var it = Items[i];
            try
            {
                if (!ui && (it.Place & EPlace.In3d) == EPlace.No) continue;
                if (ui && (it.Place & EPlace.InUI) == EPlace.No) continue;
                if (hyper && (it.Place & EPlace.InHyper) == EPlace.No) continue;
                if (dialog && (it.Place & EPlace.InDialog) == EPlace.No) continue;
                //switch (it.Place)
                //{
                //    case EPlace.In3d: if (!ui) break; continue;
                //    case EPlace.InUI: if (ui && !dialog) break; continue;
                //    case EPlace.InUIOr3d: if (!dialog) break; continue;
                //    case EPlace.InDialog: if (dialog) break; continue;
                //    case EPlace.InHyper: if (hyper) break; continue;
                //    case EPlace.Any: break;
                //    default: CLog.Error($"TryDo: {it}"); continue;
                //}
                Items.RemoveAt(i);
                it.Action();
                return it;
            }
            catch (Exception e)
            {
                CLog.Error($"TryDo: {it}, error: {e}");
            }
        }
        return null;
    }
}
