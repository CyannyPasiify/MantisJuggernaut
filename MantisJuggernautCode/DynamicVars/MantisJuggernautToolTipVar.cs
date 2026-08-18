using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MantisJuggernaut.DynamicVars;

public abstract class MantisJuggernautToolTipVar : DynamicVar
{
    public MantisJuggernautToolTipVar(string name, decimal baseValue) : base(name, baseValue)
    {
        var entry = StringHelper.Slugify(Entry.ModId) + "_" + StringHelper.Slugify(name);
        this.WithSharedTooltip(entry);
    }
}