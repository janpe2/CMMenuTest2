using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

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

        public abstract void Create(IIndirectObjectCreator creator);

        public abstract void Write(Stream stream);

        protected static PDFName CreatePostScriptFontName(Typeface typeface)
        {
            string name = typeface.FontFamily.Source.Replace(" ", "");
            FontStyle style = typeface.Style;
            FontWeight weight = typeface.Weight;

            if (style != FontStyles.Normal || weight != FontWeights.Normal)
            {
                string styleString = (style == FontStyles.Normal) ? "" : style.ToString();
                string weightString = (weight == FontWeights.Normal) ? "" : weight.ToString();
                name = $"{name}-{weightString}{styleString}";
            }

            // Add a random prefix of uppercase letters, like "FRDVGH+", which indicates this font is a subset.
            char[] chars = new char[7];
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                chars[i] = (char)random.Next('A', 'Z');
            }
            chars[6] = '+';

            return PDFName.GetEscapedName(new string(chars) + name);
        }

        internal int ResourceKeyId { get; set; }

    }
}
