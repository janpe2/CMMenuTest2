using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PDFLibrary.Font
{
    public abstract class PDFFont
    {
        protected HashSet<char> subsetCharacters = new HashSet<char>();
        protected int firstChar = 255;
        protected int lastChar = 0;
        protected PDFDictionary fontDictionary;
        protected PDFDictionary fontDescriptor;
        protected PDFStream fontStream;

        protected PDFFont(PDFDictionary fontDictionary)
        {
            this.fontDictionary = fontDictionary;
        }

        public virtual void AddStringToSubset(string str)
        {
            foreach (char ch in str)
            {
                AddCharacterToSubset(ch);
            }
        }

        public virtual void AddCharacterToSubset(char ch)
        {
            subsetCharacters.Add(ch);

            if (ch < firstChar)
            {
                firstChar = ch;
            }
            if (ch > lastChar)
            {
                lastChar = ch;
            }
        }

        public PDFDictionary GetFontDictionary()
        {
            return fontDictionary;
        }

        public abstract void Create(PDFCreator creator);

        public abstract void Write(Stream stream);
    }
}
