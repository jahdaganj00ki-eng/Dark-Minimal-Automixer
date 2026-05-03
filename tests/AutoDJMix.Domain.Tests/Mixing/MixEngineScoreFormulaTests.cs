using AutoDJMix.Domain.Mixing;
using Xunit;

namespace AutoDJMix.Domain.Tests.Mixing;

public sealed class MixEngineScoreFormulaTests
{
    [Fact]
    public void Compute_matches_spec_weights()
    {
        var score = MixEngineScoreFormula.ComputeFinalScore(baseCompat: 80, phaseBoost: 10, riskPenalty: -20);
        Assert.Equal(80 * 0.6 + 10 * 0.2 + (-20) * 0.2, score, 6);
    }
}
