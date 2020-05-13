using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PDFLibrary.Types
{
    public abstract class PDFObject
    {
        public const int DirectObject = -1;

        public virtual bool IsIndirect { get; set; }

        public abstract override string ToString();

        public virtual void Write(Stream output)
        {
            WriteASCIIBytes(ToString(), output);
        }

        public static void WriteASCIIBytes(string str, Stream output)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            output.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns an indirect reference using the PDF <c>R</c> syntax.
        /// Classes that override IsIndirect must also override this method.
        /// </summary>
        /// <returns>R syntax string</returns>
        public virtual string ToReferenceString()
        {
            return "";
        }
    }

}
