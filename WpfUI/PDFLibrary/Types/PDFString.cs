using System;
using System.Collections.Generic;
using System.Text;

namespace WpfUI.PDFLibrary.Types
{
    public class PDFString : PDFObject
    {
        public string Value { get; }

        public PDFString(string str)
        {
            this.Value = str;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');

            foreach (char ch in Value)
            {
                switch (ch)
                {
                    case '(':
                        sb.Append("\\(");
                        break;
                    case ')':
                        sb.Append("\\)");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    default:
                        if (ch >= 32 && ch <= 126)
                        {
                            sb.Append(ch);
                        }
                        else
                        {
                            string octal = System.Convert.ToString(ch & 0xFF, 8);
                            sb.Append('\\');
                            if (octal.Length < 3)
                            {
                                // Pad with '0's to get three octal digits
                                sb.Append(new string('0', 3 - octal.Length));
                            }
                            sb.Append(octal);
                        }
                        break;
                }
            }

            sb.Append(')');
            return sb.ToString();
        }

    }
}
