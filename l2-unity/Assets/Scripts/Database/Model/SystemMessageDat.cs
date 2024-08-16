public class SystemMessageDat
{
    private int _id;
    private string _message;
    private int _group;
    private string _color;

    public int Id { get { return _id; } set { _id = value; } }
    public string Message { get { return _message; } set { _message = value; } }
    public int Group { get { return _group; } set { _group = value; } }
    public string Color { get { return _color; } set { _color = value; } }
}
