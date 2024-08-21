#if FALSE
public class ShortCutChildrenModel
{
    private int[] id_skill;
    private string[] img_path;
    public ShortCutChildrenModel(int[] id_skill, string[] img_path)
    {
        this.id_skill = id_skill;
        this.img_path = img_path;
    }

    public int GetRowSkillId(int row)
    {
        return id_skill[row];
    }

    public string GetRowImgPath(int row)
    {
        return img_path[row];
    }
}
#endif
