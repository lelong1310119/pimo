namespace PismoWebInput.Core.Infrastructure.Common.Sequence;

public static class Uuid
{
    private static SequentialGuidProvider _provider;

    static Uuid()
    {
        Provider = new SequentialGuidProvider();
    }

    public static SequentialGuidProvider Provider
    {
        get => _provider;
        set => _provider = value ?? throw new SystemException("Cannot set Uuid to null");
    }

    public static Guid NewId()
    {
        return Provider.NewId();
    }

    public static string NewIdString()
    {
        return NewId().ToString();
    }
}