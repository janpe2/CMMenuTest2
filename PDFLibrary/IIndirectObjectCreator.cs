using PDFLibrary.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDFLibrary
{
    public interface IIndirectObjectCreator
    {
        public PDFArray CreateIndirectArray();

        public PDFArray CreateIndirectArray(params double[] realValues);

        public PDFArray CreateIndirectArray(params int[] intValues);

        public PDFStream CreateStream(PDFStream.Filter filter);

        public PDFDictionary CreateIndirectDictionary();


    }
}
