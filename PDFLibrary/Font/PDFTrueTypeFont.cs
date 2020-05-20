using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace PDFLibrary.Font
{
    public class PDFTrueTypeFont : PDFFont
    {
        private HashSet<char> subsetCharacters = new HashSet<char>();
        private Typeface typeface;

        public PDFTrueTypeFont(Typeface typeface)
        {
            this.typeface = typeface;
        }

        public PDFTrueTypeFont(FontFamily fontFamily, FontStyle style, FontWeight weight, FontStretch stretch)
        {
            this.typeface = new Typeface(fontFamily, style, weight, stretch);
        }

        public override void AddCharacterToSubset(char ch)
        {
            subsetCharacters.Add(ch);
        }

        private void CheckEmbeddingRights(GlyphTypeface glyphTypeface)
        {
            switch (glyphTypeface.EmbeddingRights)
            {
                case FontEmbeddingRight.Installable:
                case FontEmbeddingRight.Editable:
                case FontEmbeddingRight.PreviewAndPrint:
                    // OK
                    break;
                case FontEmbeddingRight.InstallableButNoSubsetting:
                case FontEmbeddingRight.InstallableButWithBitmapsOnly:
                case FontEmbeddingRight.InstallableButNoSubsettingAndWithBitmapsOnly:
                case FontEmbeddingRight.EditableButNoSubsetting:
                case FontEmbeddingRight.EditableButWithBitmapsOnly:
                case FontEmbeddingRight.EditableButNoSubsettingAndWithBitmapsOnly:
                case FontEmbeddingRight.RestrictedLicense:
                case FontEmbeddingRight.PreviewAndPrintButNoSubsetting:
                case FontEmbeddingRight.PreviewAndPrintButWithBitmapsOnly:
                case FontEmbeddingRight.PreviewAndPrintButNoSubsettingAndWithBitmapsOnly:
                    throw new Exception($"Font {typeface.FontFamily.Source} does not allow embedding");
            }
        }

        private GlyphTypeface CreateGlyphTypeface()
        {
            GlyphTypeface glyphTypeface = null;
            bool ok = typeface.TryGetGlyphTypeface(out glyphTypeface);
            if (!ok)
            {
                throw new Exception($"Failed to get GlyphTypeface for font {typeface.FontFamily.Source}");
            }
            return glyphTypeface;
        }

        private string CreatePostScriptFontName()
        {
            return ""; // TODO
        }

        public override void Write(Stream stream, PDFCreator creator)
        {
            string postScriptFontName = CreatePostScriptFontName();
            FontStyle style = typeface.Style;
            double italicAngle = 
                (style == FontStyles.Italic || style == FontStyles.Oblique) ? -12.0 : 0.0; // TODO just a guess

            GlyphTypeface glyphTypeface = CreateGlyphTypeface();

            PDFDictionary fontDescr = new PDFDictionary(creator.GetNextObjectNumber());
            fontDescr.Put("Type", new PDFName("FontDescriptor"));
            fontDescr.Put("FontName", new PDFName(postScriptFontName));
            fontDescr.Put("Flags", new PDFInt(4));
            fontDescr.Put("StemV", new PDFInt(100));
            fontDescr.Put("MissingWidth", new PDFInt(250));
            // fontDescr.Put("FontBBox", ); // TODO required!
            fontDescr.Put("ItalicAngle", new PDFReal(italicAngle));
            fontDescr.Put("Ascent", new PDFReal(1000 * glyphTypeface.Baseline));
            fontDescr.Put("Descent", new PDFReal(-250.0)); // TODO just a guess
            fontDescr.Put("CapHeight", new PDFInt((int)(1000 * glyphTypeface.CapsHeight)));

            PDFDictionary fontDict = new PDFDictionary(creator.GetNextObjectNumber());
            fontDescr.Put("Type", new PDFName("Font"));
            fontDescr.Put("Subtype", new PDFName("TrueType"));
            fontDescr.Put("BaseFont", new PDFName(postScriptFontName));
            // fontDescr.Put("Encoding", new PDFName("TrueType")); // omit Encoding
            fontDict.Put("FontDescriptor", fontDescr);
            // fontDict.Put("Widths", ); // TODO required!
            // fontDict.Put("FirstChar", new PDFInt()); // TODO required!
            // fontDict.Put("LastChar", new PDFInt()); // TODO required!

            PDFStream fontStream = new PDFStream(creator.GetNextObjectNumber(), PDFStream.Filter.Flate);
            fontDescr.Put("Fontfile2", fontStream);
            byte[] data = CreateSubsetData(glyphTypeface);
            fontStream.Data.Write(data, 0, data.Length);
            fontStream.StreamDictionary.Put("Length1", new PDFInt(data.Length));

            // TODO

        }

        private byte[] CreateSubsetData(GlyphTypeface glyphTypeface)
        {
            CheckEmbeddingRights(glyphTypeface);
            List<ushort> glyphIndices = new List<ushort>(subsetCharacters.Count);
            IDictionary<int, ushort> cmap = glyphTypeface.CharacterToGlyphMap;

            foreach (char ch in subsetCharacters)
            {
                ushort gid;
                if (cmap.TryGetValue((int)ch, out gid))
                {
                    glyphIndices.Add(gid);
                }
            }

            return glyphTypeface.ComputeSubset(glyphIndices);
        }
    }
}
