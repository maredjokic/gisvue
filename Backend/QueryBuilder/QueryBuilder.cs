using Backend_App.Models;
using Microsoft.Extensions.Logging;
using SqlKata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backend_App.QueryBuilder
{
    public static class QueryBuilder
    {
        public static string DateTimeQuery(DateTimeWhere dateTimeWhere)
        {
            string where = string.Empty;
            switch (dateTimeWhere.Sign)
            {
                case Sign.Equal:
                    where = $" VP.\"DateTime\" = {dateTimeWhere.From.ToString("yyyy-MM-dd HH:mm:ss")} ";
                    break;
                case Sign.NotEqual:
                    where = $" VP.\"DateTime\" != {dateTimeWhere.From.ToString("yyyy-MM-dd HH:mm:ss")} ";
                    break;
                case Sign.GreaterThan:
                    where = $" VP.\"DateTime\" > {dateTimeWhere.From.ToString("yyyy-MM-dd HH:mm:ss")} ";
                    break;
                case Sign.LessThan:
                    where = $" VP.\"DateTime\" < {dateTimeWhere.From.ToString("yyyy-MM-dd HH:mm:ss")}";
                    break;
                case Sign.Between:
                    where = $"  VP.\"DateTime\" < {dateTimeWhere.From.ToString("yyyy-MM-dd HH:mm:ss")} AND VP.\"DateTime\" > {dateTimeWhere.To.ToString("yyyy-MM-dd HH:mm:ss")} ";                  
                    break;
            }
            return where;
        }

        public static string St_Distance(string Geomerty1, string Geomerty2, int? LessThenInMeters)
        {
            if (LessThenInMeters != null)
                return $"ST_Distance({Geomerty1}, {Geomerty2}) < {(int)LessThenInMeters}";
            else
                return $"ST_Distance({Geomerty1}, {Geomerty2})";
        }


        public static string Linestring(List<Point> points)
        {
            StringBuilder stringBuilder = new StringBuilder("ST_Transform(ST_GeomFromText('LINESTRING(");
            for (int i = 0; i < points.Count; i++)
            {
                if (i != points.Count - 1)
                    stringBuilder.Append($"{points[i].Lat} {points[i].Lon}, ");
                else
                    stringBuilder.Append($"{points[i].Lat} {points[i].Lon})', 4326), 2163)");
            }

            return stringBuilder.ToString();
        }
    }

    public class DateTimeWhere
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public Sign Sign { get; set; }
    }

    public enum Sign
    {
        Equal = 0,
        NotEqual = 1,
        LessThan = 2,
        GreaterThan = 3,
        Between = 4
    }
}
