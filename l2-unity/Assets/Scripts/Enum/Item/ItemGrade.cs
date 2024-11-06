
public enum ItemGrade : byte
{
    none = 0,
    d = 1,
    c = 2,
    b = 3,
    a = 4,
    s = 5
}

public class ItemGradeParser {
    public static ItemGrade Parse(string grade) {
        switch (grade) {
            case "none":
                return ItemGrade.none;
            case "d":
                return ItemGrade.d;
            case "c":
                return ItemGrade.c;
            case "b":
                return ItemGrade.b;
            case "a":
                return ItemGrade.a;
            case "s":
                return ItemGrade.s;
            default:
                return ItemGrade.none;
        }
    }
}
