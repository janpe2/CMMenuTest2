using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFLibrary.Font
{
    public class PDFTrueTypeFont : PDFFont
    {
        private HashSet<char> subsetCharacters = new HashSet<char>();

        public PDFTrueTypeFont()
        {
            //
        }

        public override void AddCharacterToSubset(char ch)
        {
            subsetCharacters.Add(ch);
        }

        public override void Write(Stream stream)
        {
            // TODO
        }
    }
}
