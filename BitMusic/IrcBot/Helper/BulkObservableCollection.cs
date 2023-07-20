using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BitMusic.IrcBot.Helper;

public class BulkObservableCollection<T> : ObservableCollection<T>
{
    private bool _deferNotification;

    public void AddRange(IEnumerable<T> collection)
    {
        _deferNotification = true;
        foreach (T itm in collection)
        {
            Add(itm);
        }

        _deferNotification = false;
        OnCollectionChanged(
            new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized
                .NotifyCollectionChangedAction.Reset));
    }

    public void RemoveRange(IEnumerable<T> collection)
    {
        _deferNotification = true;
        foreach (T itm in collection)
        {
            Remove(itm);
        }

        _deferNotification = false;
        OnCollectionChanged(
            new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized
                .NotifyCollectionChangedAction.Reset));
    }

    public void Set(T item) => Set(new[] { item });

    public void Set(IEnumerable<T> collection)
    {
        _deferNotification = true;

        Clear();
        foreach (T itm in collection)
        {
            Add(itm);
        }

        _deferNotification = false;
        OnCollectionChanged(
            new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized
                .NotifyCollectionChangedAction.Reset));
    }

    protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (!_deferNotification)
        {
            base.OnCollectionChanged(e);
        }
    }
}
