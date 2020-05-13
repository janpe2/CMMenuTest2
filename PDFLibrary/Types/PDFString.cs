using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF data type <c>string</c>.
    /// </summary>
    public class PDFString : PDFObject
    {
        public string Value { get; }

        public PDFString(string str)
        {
            this.Value = str;
        }

        public override string ToString()
        {
            // We can't allow binary output because the returned string will end up to
            // PDFObject.WriteASCIIBytes(), which won't allow binary text.
            const bool allowBinaryOutput = false;

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
                        if (allowBinaryOutput || (ch >= 32 && ch <= 126))
                        {
                            sb.Append(ch);
                        }
                        else
                        {
                            // To avoid a binary byte, convert to an octal escape.
                            string octal = System.Convert.ToString(ch & 0xFF, 8);
                            sb.Append('\\');
                            if (octal.Length < 3)
                            {
                                // Pad with '0's to get three octal digits.
                                octal = octal.PadLeft(3, '0');
                            }
                            sb.Append(octal);
                        }
                        break;
                }
            }

            sb.Append(')');
            return sb.ToString();
        }

        public static PDFString CreateFileIDString()
        {
            // Let's create a string of random bytes instead of using the
            // MD5 algorithm that is described in the PDF spec.
            const int n = 16;
            char[] array = new char[n];
            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                array[i] = (char)random.Next(0, 256);
            }

            return new PDFString(new string(array));
        }

    }
}
