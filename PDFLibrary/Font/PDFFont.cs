using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFLibrary.Font
{
    public abstract class PDFFont
    {
        public abstract void AddCharacterToSubset(char ch);

        public abstract void Write(Stream stream, PDFCreator creator);
    }
}
