namespace PismoWebInput.Core.Infrastructure.Extensions
{
    /// <summary> Enum Extension Methods </summary>
    /// <typeparam name="T"> type of Enum </typeparam>
    public class Enum<T> where T : struct, IConvertible
    {
        public static List<string> ToNameList
        {
            get
            {
                if (!typeof(T).IsEnum)
                    throw new ArgumentException("T must be an enumerated type");

                return Enum.GetNames(typeof(T)).ToList();
            }
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }
    }
}
