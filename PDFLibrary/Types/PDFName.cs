using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF object type <c>name</c>.
    /// </summary>
    public class PDFName : PDFObject
    {
        public string Name { get; }

        public PDFName(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            // TODO Escape illegal characters, like whitespace.
            return "/" + Name;
        }

        public static PDFName GetEscapedName(string name)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0, len = name.Length; i < len; i++)
            {
                char c = name[i];
                switch (c)
                {
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                    case '/':
                    case '%':
                    case '#':
                        appendHexEscape(c, sb);
                        break;
                    default:
                        if (c <= 32)
                        {
                            appendHexEscape(c, sb);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return new PDFName(sb.ToString());
        }

        private static void appendHexEscape(int c, StringBuilder sb)
        {
            c &= 0xFF;
            sb.Append('#').Append(c.ToString("X2"));
        }
    }
}
