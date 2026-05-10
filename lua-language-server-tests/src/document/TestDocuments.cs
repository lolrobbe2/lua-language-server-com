using src.proto.document;
using System;
using System.Collections.Generic;
using System.Text;

namespace src.document
{
    internal static class TestDocuments
    {
        private static readonly Random _random = new Random();

        public static TextDocument LuaDocument()
        {
            return new TextDocument
            {
                uri = "file:///test.lua",
                languageId = "lua",
                version = 1,
                text =
                """
            local Player = {}

            function Player:new(name)
                local instance = {}
                instance.name = name
                return instance
            end

            return Player
            """
            };
        }
        public static IEnumerable<TextDocument[]> LuaDocuments()
        {
            for (int i = 0; i < 20; i++)
            {
                yield return new TextDocument[]
                {
                    CreateRandomLuaDocument(i)
                };
            }
        }

        private static TextDocument CreateRandomLuaDocument(int index)
        {
            int lines = _random.Next(5, 30);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < lines; i++)
            {
                sb.AppendLine($"local v{index}_{i} = {_random.Next(0, 1000)}");
            }

            sb.AppendLine($"return v{index}_0");

            return new TextDocument
            {
                uri = $"file:///rand{index}.lua",
                languageId = "lua",
                version = 1,
                text = sb.ToString()
            };
        }
    }
}
