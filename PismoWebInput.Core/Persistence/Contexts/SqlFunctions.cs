using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace PismoWebInput.Core.Persistence.Contexts;

public static class SqlFunctions
{
    [DbFunction("JSON_VALUE", IsBuiltIn = true)]
    public static string JsonValue(string? column, [NotParameterized] string? path)
    {
        throw new NotSupportedException();
    }
}