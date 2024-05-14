using System.Collections;
using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetModifier(Stat stat);
        IEnumerable<float> GetPercentageModifier(Stat stat);
    }
}