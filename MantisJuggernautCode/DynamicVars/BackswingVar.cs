using MegaCrit.Sts2.Core.Helpers;
using STS2RitsuLib.Cards.DynamicVars;

namespace MantisJuggernaut.DynamicVars;

public class BackswingVar : MantisJuggernautToolTipVar
{
    public const string Key = "Backswing";

    public BackswingVar(bool value) : base(Key, Convert.ToDecimal(value))
    {
        UpdateHoverTip();
    }

    public BackswingVar(string name, bool value) : base(name, Convert.ToDecimal(value))
    {
        UpdateHoverTip();
    }

    public bool BoolVal
    {
        get => Convert.ToBoolean(BaseValue);
        set
        {
            BaseValue = Convert.ToDecimal(value);
            UpdateHoverTip();
        }
    }

    public void UpdateHoverTip()
    {
        var entry = StringHelper.Slugify(Entry.ModId) + "_" +
                    StringHelper.Slugify(Key) + "_" +
                    (BoolVal ? "RIGHT" : "LEFT");
        this.WithSharedTooltip(entry);
    }
}