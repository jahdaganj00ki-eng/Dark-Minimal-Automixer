namespace AutoDJMix.Domain.Mixing;

public static class MixEngineScoreFormula
{
    public static double ComputeFinalScore(double baseCompat, double phaseBoost, double riskPenalty)
        => (baseCompat * 0.6) + (phaseBoost * 0.2) + (riskPenalty * 0.2);
}
