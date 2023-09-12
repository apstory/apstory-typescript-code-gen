using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apstory.TypescriptCodeGen.Swagger.Util
{
    public static class CasingExtensions
    {
        public static string ToCamelCase(this string input)
        {
            if (char.IsUpper(input[0]))
                input = input[0].ToString().ToLower() + input.Substring(1);

            return input;
        }

        public static string ToPascalCase(this string input)
        {
            if (char.IsUpper(input[0]))
                return input;

            if (input.Length < 1)
                return input.ToUpper();

            input = input[0].ToString().ToUpper() + input.Substring(1, input.Length - 1);

            if (char.IsUpper(input[1]))
            {
                int count = 1;
                while (count < input.Length && char.IsUpper(input[count]))
                {
                    var chr = input[count];
                    input = input.Remove(count, 1);
                    input = input.Insert(count, chr.ToString().ToLower());
                    count++;
                }
            }

            return input;
        }

        public static string ToKebabCase(this string input)
        {
            for (int i = 65; i < 90; i++)
            {
                var toReplace = (char)i;
                var newChar = (char)(i + 32);
                input = input.Replace(toReplace.ToString(), "-" + newChar.ToString());
            }

            while (input[0] == '-')
                input = input.Substring(1, input.Length - 1);

            //If the entire name is an acronym, remove the dashes
            int count = 2;
            bool didRemove = false;
            while (count < input.Length)
            {
                if (input[count] == '-')
                    if (input[count - 2] == '-')
                    {
                        input = input.Remove(count - 2, 1);
                        didRemove = true;
                    }
                count++;
            }

            if (didRemove && input[input.Length - 2] == '-')
                input = input.Remove(input.Length - 2, 1);

            return input;
        }
    }
}
