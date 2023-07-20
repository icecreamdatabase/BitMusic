using System;
using System.Globalization;
using System.Xml.Serialization;

namespace BitMusic.Settings;

public class XmlTypeSetting
{
    [XmlElement("Up")]
    public int Up;

    [XmlElement("Down")]
    public int Down;

    [XmlElement("Max")]
    public int Max;

    [XmlElement("Min")]
    public int Min;

    [XmlArray("Steps")]
    [XmlArrayItem("Step")]
    public double[] Steps = Array.Empty<double>();

    [XmlIgnore]
    public string StepsString
    {
        get => string.Join(',', Steps);
        set => Steps = GetSteps(value);
    }

    private static double[] GetSteps(string stepsString)
    {
        string[] split = stepsString.Split(',');
        double[] steps = new double[split.Length];
        for (int i = 0; i < split.Length; i++)
        {
            if (double.TryParse(split[i].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double step))
                steps[i] = step;
            else
                // Once we have 1 error in the parsing fallback to nothing
                return Array.Empty<double>();
        }

        Array.Sort(steps);
        return steps;
    }

    public double GetNextStepValue(double currentStepValue)
    {
        if (Steps.Length == 0)
            return 0d;

        int currentStepIndex = Array.IndexOf(Steps, currentStepValue);
        int nextStepIndex = Math.Min(currentStepIndex + 1, Steps.Length - 1);
        return Steps[nextStepIndex];
    }

    public double GetPreviousStepValue(double currentStepValue)
    {
        if (Steps.Length == 0)
            return 0d;

        int currentStepIndex = Array.IndexOf(Steps, currentStepValue);
        int nextStepIndex = Math.Max(currentStepIndex - 1, 0);
        return Steps[nextStepIndex];
    }

    public double GetMaxStepValue() =>
        Steps.Length > 0
            ? Steps[^1]
            : 0d;

    public double GetMinStepValue() =>
        Steps.Length > 0
            ? Steps[0]
            : 0d;
}
