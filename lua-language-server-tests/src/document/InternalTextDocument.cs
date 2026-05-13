using src.proto.document;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace src.document
{
    public class InternalTextDocumentTests
    {
    
        /*
        [Theory]
        [InlineData(-1)]
        [InlineData(1000)]
        public void GetLineShouldThrowWhenIndexIsOutOfRange(int line)
        {
            InternalTextDocument internalDoc = new InternalTextDocument(TestDocuments.LuaDocument());

            Assert.Throws<IndexOutOfRangeException>(
                () => internalDoc.GetLine(line));
        }
        */
        [Theory]
        [MemberData(nameof(TestDocuments.LuaDocuments), MemberType = typeof(TestDocuments))]
        internal void GetTextRandomDocumentsReturnsSameText(TextDocument doc)
        {
            InternalTextDocument internalDoc = new InternalTextDocument(doc);

            string result = internalDoc.GetText();

            Assert.Equal(doc.text, result);
        }
        
    }
}
