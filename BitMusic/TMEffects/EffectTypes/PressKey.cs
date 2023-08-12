using BitMusic.Settings;
using BitMusic.TMEffects.EffectHelper;

namespace BitMusic.TMEffects.EffectTypes;

public class PressKey : EffectBase
{
    public string AhkKeyCode { get; }

    public PressKey(string displayName, bool enabled, uint weight, string ahkKeyCode) : base(displayName, enabled, weight)
    {
        AhkKeyCode = ahkKeyCode;
    }

    public override void Execute(XmlTmSettings tmSettings)
    {
        string code = $$"""
                        #IfWinActive ahk_exe {{tmSettings.ProcessName}}
                        Send , {{{AhkKeyCode}}}
                        """;

        AhkHelper.ExecuteAhkScript(code);
    }
}
