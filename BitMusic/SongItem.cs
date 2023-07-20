using System.IO;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BitMusic;

public class SongItem : ObservableRecipient
{
    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    private Brush _background;

    public Brush Background
    {
        get => _background;
        private set => SetProperty(ref _background, value);
    }

    private string _fileName = string.Empty;

    public string FileName
    {
        get => _fileName;
        private set => SetProperty(ref _fileName, value);
    }

    public readonly FileInfo FileInfo;

    public SongItem(string path)
    {
        FileInfo = new FileInfo(path);

        FileName = FileInfo.Name[..^FileInfo.Extension.Length];
    }
    
    //protected bool Equals(SongItem other)
    //{
    //    return FileInfo.FullName.Equals(other.FileInfo.FullName);
    //}

    //public override bool Equals(object? obj)
    //{
    //    if (ReferenceEquals(null, obj)) 
    //        return false;
    //    if (ReferenceEquals(this, obj)) 
    //        return true;
    //    if (obj.GetType() != this.GetType()) 
    //        return false;
    //    return Equals((SongItem)obj);
    //}

    //public override int GetHashCode()
    //{
    //    return FileInfo.FullName.GetHashCode();
    //}

}
