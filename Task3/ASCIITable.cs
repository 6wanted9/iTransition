using ConsoleTableExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Task3
{
    static class ASCIITable
    {
       
        public static void CreateTable(Rules rules)
        {
            var names = rules.GetElements();
            var grid = rules.GetRules();
            int len = names.Length;
            var list = new List<List<object>>();
            var topic = names.Cast<object>().ToList();
            topic.Insert(0, "PLAYER\\PC");
            list.Add(topic);
            for (int i = 0; i < names.Length; i++)
            {
                object[] row = Enumerable.Range(0, len).Select(colNum => (object)grid[i, colNum]).ToArray();
                var listCol = row.Cast<object>().ToList();
                listCol.Insert(0, names[i]);
                list.Add(listCol);
            }
            ConsoleTableBuilder.From(list).WithFormat(ConsoleTableBuilderFormat.Alternative).ExportAndWriteLine();
        }
    }
}
