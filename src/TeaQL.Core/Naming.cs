using System.Text;

namespace TeaQL.Core;

public static class Naming
{
    public static string DefaultTableName(string entityName)
    {
        var outBuilder = new StringBuilder(entityName.Length + 5);
        for (int index = 0; index < entityName.Length; index++)
        {
            char ch = entityName[index];
            if (char.IsUpper(ch))
            {
                if (index > 0)
                {
                    outBuilder.Append('_');
                }
                outBuilder.Append(char.ToLowerInvariant(ch));
            }
            else
            {
                outBuilder.Append(ch);
            }
        }
        outBuilder.Append("_data");
        return outBuilder.ToString();
    }
}
