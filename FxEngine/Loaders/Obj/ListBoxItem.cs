namespace FxEngine.Loaders.OBJ
{
    public class ListBoxItem
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public object Tag { get; set; }
    }
}