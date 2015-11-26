using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HorseLeague.Helpers
{
    public class HTMLTableParser : HorseLeague.Helpers.IHTMLTableParser
    {
        public void FindTable(string HTML, Func<string, bool> callback)
        {
            const string TABLE = "<table";
            const string END_TABLE = "</table>";

            parseItAll(HTML, TABLE, END_TABLE, callback);
        }

        public void ParseTableRow(string table, Action<string> callback)
        {
            const string TR = "<tr>";
            const string END_TR = "</tr>";

            parseItAll(table, TR, END_TR, (string row) =>
            {
                callback(row);
                return false; 
            });
        }

        public void ParseTableCell(string table, Action<string> callback)
        {
            const string TR = "<td";
            const string END_TR = "</td>";

            parseItAll(table, TR, END_TR, (string cell) =>
            {
                callback(cell);
                return false;
            });
        }

        public T Mid<T>(string toSearch, string begin, string end)
        {
            int beginIndex = toSearch.IndexOf(begin);
            
            return (T)Convert.ChangeType(toSearch.Substring(
                (beginIndex + begin.Length),
                (toSearch.IndexOf(end, beginIndex) - (beginIndex + end.Length - 1))),
                typeof(T));
        }

        public bool InnerTextContains(string toSearch, string begin)
        {
            return toSearch.IndexOf(begin) > 0;
        }
        public T InnerText<T>(string toSearch, string begin, string end)
        {

            int beginPos = toSearch.IndexOf(begin);

            if (beginPos > 0)
            {
                string filtered = toSearch.Substring(beginPos);

                int beginIndex = filtered.IndexOf(begin);
                int innerTag = filtered.IndexOf(">");
                int endTag = filtered.IndexOf(end, innerTag);
                string theVal = filtered.Substring(
                    (innerTag + 1),
                    endTag - innerTag - 1);

                return (T)Convert.ChangeType(theVal,
                    typeof(T));
            }

            throw new InvalidOperationException();
        }

        private void parseItAll(string toBeParsed, string beginTag, 
            string endTag, Func<string, bool> callback)
        {
            int i = toBeParsed.IndexOf(beginTag);

            while (i > 0)
            {
                int endTagPos = toBeParsed.IndexOf(endTag, i);
                if (callback(toBeParsed.Substring(i, endTagPos - i))) break;

                i = toBeParsed.IndexOf(beginTag, endTagPos);
            }
        }
    }
}
