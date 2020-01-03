namespace BASApi.CSharp.Interfaces
{
    public interface IFunctionRunner
    {
        IBasFunction RunFunction(string functionName, params (string Key, object Value)[] functionParameters);
    }
}