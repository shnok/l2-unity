
public enum ItemMaterial : byte
{
    crystal = 0,
    steel = 1,
    fine_steel = 2,
    blood_steel = 3,
    bronze = 4,
    silver = 5,
    gold = 6,
    mithril = 7,
    oriharukon = 8,
    paper = 9,
    wood = 10,
    cloth = 11,
    leather = 12,
    bone = 13,
    horn = 14,
    damascus = 15,
    adamantite = 16,
    chrysolite = 17,
    liquid = 18,
    scale_of_dragon = 19,
    dyestuff = 20,
    cobweb = 21,
    seed = 22
}

public class ItemMaterialParser {
    public static ItemMaterial Parse(string material) {
        switch (material) {
            case "crystal":
                return ItemMaterial.crystal;
            case "steel":
                return ItemMaterial.steel;
            case "fine_steel":
                return ItemMaterial.fine_steel;
            case "blood_steel":
                return ItemMaterial.blood_steel;
            case "bronze":
                return ItemMaterial.bronze;
            case "silver":
                return ItemMaterial.silver;
            case "gold":
                return ItemMaterial.gold;
            case "mithril":
                return ItemMaterial.mithril;
            case "oriharukon":
                return ItemMaterial.oriharukon;
            case "paper":
                return ItemMaterial.paper;
            case "wood":
                return ItemMaterial.wood;
            case "cloth":
                return ItemMaterial.cloth;
            case "leather":
                return ItemMaterial.leather;
            case "bone":
                return ItemMaterial.bone;
            case "horn":
                return ItemMaterial.horn;
            case "damascus":
                return ItemMaterial.damascus;
            case "adamantite":
                return ItemMaterial.adamantite;
            case "chrysolite":
                return ItemMaterial.chrysolite;
            case "liquid":
                return ItemMaterial.liquid;
            case "scale_of_dragon":
                return ItemMaterial.scale_of_dragon;
            case "dyestuff":
                return ItemMaterial.dyestuff;
            case "cobweb":
                return ItemMaterial.cobweb;
            case "seed":
                return ItemMaterial.seed;
            default:
                return ItemMaterial.crystal;
        }
    }
}
