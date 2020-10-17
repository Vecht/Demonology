namespace Demonology.Prime.Interfaces
{
    /// <summary>
    /// Any type to implement this interface should adhere to the following conventions:
    ///  * The factory should have a single, parameterless, public constructor
    ///  * The factory should not derive from any other custom types and should itself be sealed
    ///  * The factory should not record any state; ideally should not have any fields or properties
    ///  * The factory should have one or more "factory methods"; i.e. - methods which return type T and take some number of parameters as arguments
    ///  * The factory should have no other methods besides those mentioned above
    /// </summary>
    public interface IFactory<out T> : IFactoryType
    {
        T Default();
    }

    /// <summary>
    /// DO NOT USE THIS.
    /// </summary>
    public interface IFactoryType { }
}
