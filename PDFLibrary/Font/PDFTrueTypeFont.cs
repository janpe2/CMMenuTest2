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
        private Typeface typeface;

        public PDFTrueTypeFont(PDFDictionary fontDictionary, Typeface typeface) :
            base(fontDictionary)
        {
            this.typeface = typeface;
        }

        public PDFTrueTypeFont(PDFDictionary fontDictionary, FontFamily fontFamily, 
            FontStyle style, FontWeight weight, FontStretch stretch) :
            base(fontDictionary)
        {
            this.typeface = new Typeface(fontFamily, style, weight, stretch);
        }

        /// <summary>
        /// Only for testing.
        /// </summary>
        internal PDFTrueTypeFont(PDFDictionary fontDictionary) :
            base(fontDictionary)
        {
            FontFamily fontFamily = new FontFamily("Segoe Print");
            this.typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
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

        private PDFName CreatePostScriptFontName()
        {
            // Add a random prefix like "FRDVGH+" which indicates this font is a subset.

            char[] chars = new char[7];
            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                chars[i] = (char)random.Next('A', 'Z');
            }
            chars[6] = '+';

            return PDFName.GetEscapedName(new string(chars) + typeface.FontFamily.Source);
        }

        public override void Create(PDFCreator creator)
        {
            if (firstChar > lastChar)
            {
                firstChar = lastChar = 0;
            }

            PDFName postScriptFontName = CreatePostScriptFontName();
            FontStyle style = typeface.Style;
            double italicAngle =
                (style == FontStyles.Italic || style == FontStyles.Oblique) ? -12.0 : 0.0; // TODO just a guess

            GlyphTypeface glyphTypeface = CreateGlyphTypeface();
            IDictionary<int, ushort> cmap = glyphTypeface.CharacterToGlyphMap;

            fontDescriptor = creator.CreateIndirectDictionary();
            fontDescriptor.Put("Type", new PDFName("FontDescriptor"));
            fontDescriptor.Put("FontName", postScriptFontName);
            fontDescriptor.Put("Flags", new PDFInt(4)); // symbolic
            fontDescriptor.Put("StemV", new PDFInt(100));
            fontDescriptor.Put("MissingWidth", new PDFInt(250));
            fontDescriptor.Put("FontBBox", GetFontBBox(glyphTypeface, creator));
            fontDescriptor.Put("ItalicAngle", new PDFReal(italicAngle));
            fontDescriptor.Put("Ascent", new PDFReal(1000 * glyphTypeface.Baseline));
            fontDescriptor.Put("Descent", new PDFReal(-250.0)); // TODO just a guess
            fontDescriptor.Put("CapHeight", new PDFInt((int)(1000 * glyphTypeface.CapsHeight)));

            fontDictionary.Put("Type", new PDFName("Font"));
            fontDictionary.Put("Subtype", new PDFName("TrueType"));
            fontDictionary.Put("BaseFont", postScriptFontName);
            // fontDescr.Put("Encoding", ); // omit Encoding
            fontDictionary.Put("FontDescriptor", fontDescriptor);
            fontDictionary.Put("Widths", GetWidths(glyphTypeface, creator, cmap));
            fontDictionary.Put("FirstChar", new PDFInt(firstChar));
            fontDictionary.Put("LastChar", new PDFInt(lastChar));
            // fontDict.Put("ToUnicode", );

            fontStream = creator.CreateStream(PDFStream.Filter.Flate);
            fontDescriptor.Put("FontFile2", fontStream);
            byte[] data = CreateSubsetData(glyphTypeface, cmap);
            fontStream.Data.Write(data, 0, data.Length);
            fontStream.StreamDictionary.Put("Length1", new PDFInt(data.Length));
        }

        public override void Write(Stream stream)
        {
            fontDescriptor.Write(stream);
            fontDictionary.Write(stream);
            fontStream.Write(stream);
        }

        private PDFObject GetWidths(GlyphTypeface glyphTypeface, PDFCreator creator, 
            IDictionary<int, ushort> cmap)
        {
            PDFArray widths = creator.CreateIndirectArray();
            IDictionary<ushort, double> advWidths = glyphTypeface.AdvanceWidths;

            for (char ch = (char)firstChar; ch <= lastChar; ch++)
            {
                int width = 0;
                if (subsetCharacters.Contains(ch))
                {
                    ushort gid;
                    if (cmap.TryGetValue((int)ch, out gid))
                    {
                        double widthDouble;
                        if (advWidths.TryGetValue(gid, out widthDouble))
                        {
                            width = (int)(1000 * widthDouble);
                        }
                    } 
                }
                widths.Array.Add(new PDFInt(width));
            }

            return widths;
        }

        private byte[] CreateSubsetData(GlyphTypeface glyphTypeface, IDictionary<int, ushort> cmap)
        {
            CheckEmbeddingRights(glyphTypeface);
            List<ushort> glyphIndices = new List<ushort>(subsetCharacters.Count);

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

        /// <summary>
        /// Reads a 32-bit big-endian signed integer.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>integer</returns>
        private static int ReadInt32(Stream stream)
        {
            return (stream.ReadByte() << 24) | (stream.ReadByte() << 16) |
                (stream.ReadByte() << 8) | stream.ReadByte();
        }

        /// <summary>
        /// Reads a 16-bit big-endian signed integer.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>integer</returns>
        private static int ReadInt16(Stream stream)
        {
            short s = (short)((stream.ReadByte() << 8) | stream.ReadByte());
            return (int)s;
        }

        private static void ReadSfntVersion(Stream stream)
        {
            switch (ReadInt32(stream))
            {
                case 0x00010000: // 1.0 (Fixed) = TrueType Windows
                    break;
                case 0x74727565: // 'true' = TrueType Apple
                    break;
                default:
                    // TODO TTC is not supported
                    throw new IOException("Unsupported sfnt version in font");
            }
        }

        private PDFArray GetFontBBox(GlyphTypeface glyphTypeface, PDFCreator creator)
        {
            // We must parse the 'head' table of the font file to get FontBBox.

            using (Stream stream = glyphTypeface.GetFontStream())
            {
                ReadSfntVersion(stream);

                int numTables = ReadInt16(stream) & 0xFFFF;
                ReadInt16(stream); // searchRange
                ReadInt16(stream); // entrySelector
                ReadInt16(stream); // rangeShift

                int headTableOffset = -1;

                for (int i = 0; i < numTables; i++)
                {
                    int tag = ReadInt32(stream);
                    ReadInt32(stream); // checksum
                    int offset = ReadInt32(stream);
                    ReadInt32(stream); // length
                    switch (tag)
                    {
                        case 0x68656164:  // 'head'
                            headTableOffset = offset;
                            break;
                    }
                }

                if (headTableOffset == -1)
                {
                    throw new IOException("Cannot find 'head' table in font");
                }

                stream.Seek(headTableOffset + 12, SeekOrigin.Begin);
                int magic = ReadInt32(stream);
                if (magic != 0x5F0F3CF5)
                {
                    throw new IOException("Invalid magic code in 'head' table");
                }
                ReadInt16(stream); // flags
                int unitsPerEm = ReadInt16(stream) & 0xFFFF;
                stream.Seek(2 * 8, SeekOrigin.Current); // Skip creation and modification dates
                int left = ReadInt16(stream);
                int bottom = ReadInt16(stream);
                int right = ReadInt16(stream);
                int top = ReadInt16(stream);

                PDFArray fontBBox = creator.CreateIndirectArray();
                fontBBox.Array.Add(new PDFInt((int)Math.Floor(1000.0 * left / unitsPerEm)));
                fontBBox.Array.Add(new PDFInt((int)Math.Floor(1000.0 * bottom / unitsPerEm)));
                fontBBox.Array.Add(new PDFInt((int)Math.Ceiling(1000.0 * right / unitsPerEm)));
                fontBBox.Array.Add(new PDFInt((int)Math.Ceiling(1000.0 * top / unitsPerEm)));
                return fontBBox;
            }
        }
    }
}
