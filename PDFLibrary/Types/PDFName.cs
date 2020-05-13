using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary.Types
{
    /// <summary>
    /// PDF data type <c>name</c>.
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
    }
}
