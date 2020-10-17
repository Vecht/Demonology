namespace Demonology.Test.Data
{
    public class Sacrifice
    {
        public SacrificeType Type { get; }

        public Sacrifice(SacrificeType type)
        {
            Type = type;
        }
    }

    public enum SacrificeType
    {
        Goat,
        Ox,
        Human,
    }
}
