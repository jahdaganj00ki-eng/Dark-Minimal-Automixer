namespace AutoDJMix.Domain.Mixing;

public enum MixDecisionBand
{
    Avoid = 0,
    Risky = 1,
    Good = 2,
    Perfect = 3
}

public static class MixDecisionMatrix
{
    public static MixDecisionBand Classify(double finalScore)
    {
        if (finalScore >= 85) return MixDecisionBand.Perfect;
        if (finalScore >= 70) return MixDecisionBand.Good;
        if (finalScore >= 55) return MixDecisionBand.Risky;
        return MixDecisionBand.Avoid;
    }
}
