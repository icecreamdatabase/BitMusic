using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using BitMusic.Helper;
using BitMusic.ViewModel;

namespace BitMusic;

public class MusicPlayer
{
    private readonly Random _shuffleRandom = new();
    private readonly BitMusicViewModel _bitMusicViewModel;
    private readonly TextBoxLogger _textBoxLogger;
    private readonly MediaPlayer _mediaPlayer = new();
    private FileInfo? _currentFile;
    private bool _isPlaying;

    public MusicPlayer(BitMusicViewModel bitMusicViewModel, TextBoxLogger textBoxLogger)
    {
        _bitMusicViewModel = bitMusicViewModel;
        _textBoxLogger = textBoxLogger;
        _mediaPlayer.MediaEnded += MediaEnded;
        //Reload();
    }

    public double Balance
    {
        get => _mediaPlayer.Balance;
        set => _mediaPlayer.Balance = value / 100d;
    }

    public double SpeedRatio
    {
        get => _mediaPlayer.SpeedRatio;
        set
        {
            _mediaPlayer.Pause();
            _mediaPlayer.SpeedRatio = value / 100d;
            _mediaPlayer.Play();
        }
    }

    public double Volume
    {
        get => _mediaPlayer.Volume;
        set => _mediaPlayer.Volume = value / 100d;
    }

    public void Reload()
    {
        if (_currentFile != null)
        {
            bool currentSongStillExists = _bitMusicViewModel.MusicSettingsViewModel.SongList
                .Any(songItem =>
                    songItem.FileInfo.FullName.Equals(_currentFile.FullName, StringComparison.OrdinalIgnoreCase)
                );
            if (currentSongStillExists)
            {
                _mediaPlayer.Play();
                return;
            }
        }

        NextSong();
    }

    public void NextSong()
    {
        if (_bitMusicViewModel.MusicSettingsViewModel.SongList.Count == 0)
        {
            _currentFile = null;
            _isPlaying = false;
            _mediaPlayer.Stop();
            return;
        }


        int currentIndex = -1;
        if (_currentFile != null)
        {
            SongItem? currentSongItem = _bitMusicViewModel.MusicSettingsViewModel.SongList
                .FirstOrDefault(songItem =>
                    songItem.FileInfo.FullName.Equals(_currentFile.FullName, StringComparison.OrdinalIgnoreCase)
                );
            if (currentSongItem != null)
                currentIndex = _bitMusicViewModel.MusicSettingsViewModel.SongList.IndexOf(currentSongItem);
        }

        int nextIndex;
        if (_bitMusicViewModel.MainTabViewModel.RepeatCheckbox)
        {
            nextIndex = currentIndex;
        }
        else if (_bitMusicViewModel.MainTabViewModel.ShuffleCheckbox)
        {
            nextIndex = _shuffleRandom.Next(0, _bitMusicViewModel.MusicSettingsViewModel.SongList.Count);
            // Shuffle should not play the same song again.
            if (nextIndex == currentIndex)
                nextIndex++;
        }
        else
        {
            nextIndex = currentIndex + 1;
        }

        nextIndex %= _bitMusicViewModel.MusicSettingsViewModel.SongList.Count;


        PlaySong(_bitMusicViewModel.MusicSettingsViewModel.SongList[nextIndex].FileInfo);
    }

    private void PlaySong(FileInfo fileInfo)
    {
        _currentFile = fileInfo;
        _mediaPlayer.Stop();
        _mediaPlayer.Open(new Uri(fileInfo.FullName));
        _textBoxLogger.WriteLine($"🎵 Now playing: {fileInfo.Name.Replace(fileInfo.Extension, string.Empty)}");
        _mediaPlayer.Play();
        _isPlaying = true;
        //_mediaPlayer.SpeedRatio
    }

    private void MediaEnded(object? sender, EventArgs e)
    {
        _isPlaying = false;
        NextSong();
    }

    public void PlayPause()
    {
        if (_isPlaying)
        {
            _isPlaying = false;
            _mediaPlayer.Pause();
        }
        else
        {
            if (_currentFile == null)
                NextSong();
            else
            {
                _isPlaying = true;
                _mediaPlayer.Play();
            }
        }
    }
}
