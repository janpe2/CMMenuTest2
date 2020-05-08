using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WpfUI.PDFLibrary.Types
{
    public abstract class PDFObject
    {
        public abstract override string ToString();

        public virtual void Write(FileStream output)
        {
            WriteASCIIBytes(ToString(), output);
        }

        public static void WriteASCIIBytes(string str, FileStream output)
        {
            foreach (char ch in str)
            {
                output.WriteByte((byte)ch);
            }
        }

    }

}
