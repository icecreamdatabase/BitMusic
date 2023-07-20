using System;
using BitMusic.ViewModel;

namespace BitMusic.Helper;

public class TextBoxLogger
{
    private readonly BitMusicViewModel _bitMusicViewModel;

    public TextBoxLogger(BitMusicViewModel bitMusicViewModel)
    {
        _bitMusicViewModel = bitMusicViewModel;
    }

    public void WriteLine(string line)
    {
        _bitMusicViewModel.TextBoxText += $"{line}{Environment.NewLine}";
    }
}
