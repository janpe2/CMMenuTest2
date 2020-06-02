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
        private const bool IsSymbolic = false;
        private Typeface _typeface;

        public PDFTrueTypeFont(PDFDictionary fontDictionary, Typeface typeface) :
            base(fontDictionary)
        {
            this._typeface = typeface;
        }

        public PDFTrueTypeFont(PDFDictionary fontDictionary, FontFamily fontFamily, 
            FontStyle style, FontWeight weight, FontStretch stretch) :
            base(fontDictionary)
        {
            this._typeface = new Typeface(fontFamily, style, weight, stretch);
        }

        /// <summary>
        /// Only for testing.
        /// </summary>
        internal PDFTrueTypeFont(PDFDictionary fontDictionary) :
            base(fontDictionary)
        {
            FontFamily fontFamily = new FontFamily("Segoe Print");
            this._typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
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
                    throw new Exception($"Font {_typeface.FontFamily.Source} does not allow embedding");
            }
        }

        private GlyphTypeface CreateGlyphTypeface()
        {
            GlyphTypeface glyphTypeface = null;
            bool ok = _typeface.TryGetGlyphTypeface(out glyphTypeface);
            if (!ok)
            {
                throw new Exception($"Failed to get GlyphTypeface for font {_typeface.FontFamily.Source}");
            }
            return glyphTypeface;
        }

        public override void CreatePDFData(IIndirectObjectCreator creator)
        {
            if (firstChar > lastChar)
            {
                firstChar = lastChar = 0;
            }

            PDFName postScriptFontName = CreatePostScriptFontName(_typeface);
            GlyphTypeface glyphTypeface = CreateGlyphTypeface();
            IDictionary<int, ushort> cmap = glyphTypeface.CharacterToGlyphMap;
            byte[] subsetData = CreateSubsetData(glyphTypeface, cmap);

            fontDescriptor = creator.CreateIndirectDictionary();
            fontDescriptor.Put("Type", new PDFName("FontDescriptor"));
            fontDescriptor.Put("FontName", postScriptFontName);
            fontDescriptor.Put("Flags", new PDFInt(IsSymbolic ? 4 : 32));
            fontDescriptor.Put("MissingWidth", new PDFInt(250));
            AddFontMetrics(fontDescriptor, glyphTypeface, subsetData);

            fontDictionary.Put("Type", new PDFName("Font"));
            fontDictionary.Put("Subtype", new PDFName("TrueType"));
            fontDictionary.Put("BaseFont", postScriptFontName);
            if (!IsSymbolic)
            {
                fontDictionary.Put("Encoding", new PDFName("WinAnsiEncoding"));
            }
            fontDictionary.Put("FontDescriptor", fontDescriptor);
            fontDictionary.Put("Widths", GetWidths(glyphTypeface, creator, cmap));
            fontDictionary.Put("FirstChar", new PDFInt(firstChar));
            fontDictionary.Put("LastChar", new PDFInt(lastChar));
            // fontDict.Put("ToUnicode", );

            fontStream = creator.CreateStream(PDFStream.Filter.Flate);
            fontDescriptor.Put("FontFile2", fontStream);
            fontStream.Data.Write(subsetData, 0, subsetData.Length);
            fontStream.StreamDictionary.Put("Length1", new PDFInt(subsetData.Length));
        }

        public override void Write(Stream stream)
        {
            fontDescriptor.Write(stream);
            fontDictionary.Write(stream);
            fontStream.Write(stream);
        }

        private PDFObject GetWidths(GlyphTypeface glyphTypeface, IIndirectObjectCreator creator, 
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
                case 0x00010000: 
                    // 1.0 (Fixed) = TrueType Windows
                    break;
                case 0x74727565: 
                    // 'true' = TrueType Apple
                    break;
                case 0x4F54544F: 
                    // 'OTTO'
                    throw new IOException("OpenType CFF font is not supported");
                case 0x74746366:
                    // 'ttcf'
                    throw new IOException("TTC font collection is not supported");
                default:
                    throw new IOException("Unsupported sfnt version in font");
            }
        }

        private void AddFontMetrics(PDFDictionary fontDescriptor, GlyphTypeface glyphTypeface, 
            byte[] subsetData)
        {
            // We must parse the 'head' table of the font file to get FontBBox.

            using (Stream stream = new MemoryStream(subsetData))
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

                // Convert to font size 1000
                left = (int)Math.Floor(1000.0 * left / unitsPerEm);
                bottom = (int)Math.Floor(1000.0 * bottom / unitsPerEm);
                right = (int)Math.Ceiling(1000.0 * right / unitsPerEm);
                top = (int)Math.Ceiling(1000.0 * top / unitsPerEm);

                PDFArray fontBBox = new PDFArray(PDFObject.DirectObject);
                fontBBox.Array.Add(new PDFInt(left));
                fontBBox.Array.Add(new PDFInt(bottom));
                fontBBox.Array.Add(new PDFInt(right));
                fontBBox.Array.Add(new PDFInt(top));

                fontDescriptor.Put("FontBBox", fontBBox);
                fontDescriptor.Put("Descent", new PDFInt(bottom));
                fontDescriptor.Put("Ascent", new PDFInt((int)(1000 * glyphTypeface.Baseline)));
                fontDescriptor.Put("CapHeight", new PDFInt((int)(1000 * glyphTypeface.CapsHeight)));
                fontDescriptor.Put("StemV", new PDFInt(90)); // just a guess
                FontStyle style = _typeface.Style;
                double italicAngle =
                    (style == FontStyles.Italic || style == FontStyles.Oblique) ? -12.0 : 0.0; // just a guess
                fontDescriptor.Put("ItalicAngle", new PDFReal(italicAngle));
            }
        }
    }
}
