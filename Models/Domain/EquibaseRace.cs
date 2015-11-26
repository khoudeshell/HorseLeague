using HorseLeague.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace HorseLeague.Models.Domain
{
    public class EquibaseRace
    {
        private readonly Uri uri;
        private readonly string HTML;
        private IHTMLTableParser parser;

        public IList<RaceDetail> Horses { get; private set;  }

        public EquibaseRace(Uri uri) : this(uri, new WebClient().DownloadString(uri), new HTMLTableParser()) { }

        public EquibaseRace(Uri uri, string HTML, IHTMLTableParser parser )
        {
            this.uri = uri;
            this.HTML = HTML;
            this.parser = parser;

            parseOutput();
        }

        private void parseOutput()
        {
            const string START_TABLE = @"<table class=""table-hover clear entryTablePadding"">";
            
            parser.FindTable(this.HTML, (string table) =>
            {
                if (table.IndexOf(START_TABLE) != 0)
                    return false;

                Horses = new List<RaceDetail>();

                parser.ParseTableRow(table, (string table_row) =>
                {
                    if (table_row.IndexOf("<th") > 0)
                        return;

                    int col_counter = 0;
                    RaceDetail rd = new RaceDetail();

                    parser.ParseTableCell(table_row, (string cell) =>
                    {
                        if (col_counter == 0 && parser.InnerTextContains(cell, "<div"))
                        {
                                rd.PostPosition = parser.InnerText<int>(cell, "<div", "</div>");
                        }
                        if (col_counter == 2 && parser.InnerTextContains(cell, "<a"))
                        {
                            rd.Horse = new Horse()
                            {
                                Name = parser.Mid<string>(parser.InnerText<string>(cell, "<a", "</a>"), "", "(").Trim()
                            };
                        }
                        
                        col_counter++;
                    });

                    if(rd.PostPosition > 0)
                        this.Horses.Add(rd);
                });

                return true;
            });
        }
    }
}