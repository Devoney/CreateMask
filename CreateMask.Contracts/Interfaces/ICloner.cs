namespace CreateMask.Contracts.Interfaces
{
    public interface ICloner
    {
        T DeepClone<T>(T source);
    }
}
