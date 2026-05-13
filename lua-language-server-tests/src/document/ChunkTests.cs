using src.proto;
using src.proto.document;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace tests.src.document
{
    public class ChunkTests
    {
        [Fact]
        void ChunkGetPosition()
        {
            string test =
            """
            test1
            test2
            test3
            test4
            test5
            """;
            Chunk chunk = new Chunk(test);
            Position position = new Position();
            position.line = 2;
            position.character = 3;
            Chunk updatedChunk = chunk.GetPosition(position);
            string expected =
            """
            t2
            test3
            test4
            test5
            """;
            Assert.Equal(expected, updatedChunk.Text);
        }
        [Fact]
        public void Replace_SingleLineText()
        {
            Chunk chunk = new Chunk(new List<string>
            {
                "Hello World"
            });

            ProtoRange range = new ProtoRange
            {
                start = new Position { line = 0, character = 6 },
                end = new Position { line = 0, character = 11 }
            };

            Chunk result = chunk.Replace(range, new[] { "CSharp" });

            Assert.Equal("Hello CSharp", result.Text.Split("\n")[0]);
        }
        [Fact]
        public void Replace_MultiLine()
        {
            Chunk chunk = new Chunk(new List<string>
            {
                "Hello World",
                "Second Line",
                "Third Line"
            });

            ProtoRange range = new ProtoRange
            {
                start = new Position { line = 0, character = 6 },
                end = new Position { line = 1, character = 6 }
            };

            Chunk result = chunk.Replace(range, new[]
            {
                "CSharp",
                "Inserted"
            });

            Assert.Equal(3, result.Text.Split("\n").Length);
            Assert.Equal("Hello CSharp", result.Text.Split("\n")[0]);
            Assert.Equal("Inserted Line", result.Text.Split("\n")[1]);
        }
        [Fact]
        public void Replace_InsertText()
        {
            Chunk chunk = new Chunk(new List<string>
            {
                "Hello World"
            });

            ProtoRange range = new ProtoRange
            {
                start = new Position { line = 0, character = 5 },
                end = new Position { line = 0, character = 5 }
            };

            Chunk result = chunk.Replace(range, new[]
            {
                ", Beautiful"
            });

            Assert.Equal("Hello, Beautiful World", result.Text.Split("\n")[0]);
        }
    }
}
