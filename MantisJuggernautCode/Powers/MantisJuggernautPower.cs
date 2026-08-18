using STS2RitsuLib.Scaffolding.Content;

namespace MantisJuggernaut.Powers;

public abstract class MantisJuggernautPower : ModPowerTemplate
{
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        // TODO RitsuLib analyzer: 资源路径 'res://MantisJuggernaut/images/powers/MantisJuggernautPower.png' 在项目资源索引中未找到。
        $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        // TODO RitsuLib analyzer: 资源路径 'res://MantisJuggernaut/images/powers/MantisJuggernautPower.png' 在项目资源索引中未找到。
        $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}