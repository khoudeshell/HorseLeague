using System;
namespace HorseLeague.Helpers
{
    public interface IHTMLTableParser
    {
        void FindTable(string HTML, Func<string, bool> callback);
        T Mid<T>(string toSearch, string begin, string end);
        void ParseTableCell(string table, Action<string> callback);
        void ParseTableRow(string table, Action<string> callback);
        T InnerText<T>(string toSearch, string begin, string end);
        bool InnerTextContains(string toSearch, string begin);
    }
}
