﻿using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoMapperExtensions
{
    public static class AutoMapperConfig
    {
        public static MapperConfigurationExpression express = new MapperConfigurationExpression();

        public static void Init(params Action<IMapperConfigurationExpression>[] inits)
        {
            CreateMap(express);
            inits?.ToList().ForEach(i => i.Invoke(express));
            AutoMapperExtensions._mapper = new MapperConfiguration(express).CreateMapper();
        }

        public static void CreateMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<DateTime, string>().ConvertUsing(d => d.ToCommonDateTime());
            cfg.CreateMap<string, DateTime>().ConvertUsing(ConvertToDateTime);
            cfg.CreateMap<string, DateTime?>().ConvertUsing(ConvertToDateTimeNullable);
            cfg.CreateMap<DateTime, DateTime?>().ConvertUsing(ConvertToDateTimeNullable);
            cfg.CreateMap<string, bool?>().ConvertUsing(ConvertToBoolNullable);
            cfg.CreateMap<bool, string>().ConvertUsing(ConvertToString);
            cfg.CreateMap<bool?, string>().ConvertUsing(ConvertToString);
        }

        public static MapperConfiguration CreateMapNew(Action<IMapperConfigurationExpression> action)
        {
            return new MapperConfiguration(cfg =>
            {
                CreateMap(cfg);
                action.Invoke(cfg);
            });
        }

        private static DateTime ConvertToDateTime(this string s)
        {
            if (DateTime.TryParse(s, out DateTime time)) return time;
            else
            {
                if (string.IsNullOrWhiteSpace(s)) return time;
                else throw new Exception("自动转化不识别的时间格式:" + s);
            }
        }

        private static DateTime? ConvertToDateTimeNullable(this string s)
        {
            if (DateTime.TryParse(s, out DateTime time)) return time;
            else
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                else throw new Exception("自动转化不识别的时间格式:" + s);
            }
        }

        private static DateTime? ConvertToDateTimeNullable(this DateTime d)
        {
            if (d == default(DateTime)) return null;
            else return d;
        }

        private static bool? ConvertToBoolNullable(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;
            else if (s == "0")
                return false;
            else if (s == "1")
                return true;
            else
                return true;
        }

        private static string ConvertToString(this bool? b)
        {
            if (!b.HasValue)
                return string.Empty;
            else
                return ConvertToString(b.Value);
        }

        private static string ConvertToString(this bool b)
        {
            if (b)
                return "1";
            else
                return "0";
        }


        #region datetime

        public static string DateTimeType { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public static string DateType { get; set; } = "yyyy-MM-dd";

        public static bool defaultEmpty { get; set; } = true;

        private static string ToCommonDateTime(this DateTime d)
        {
            return defaultEmpty && d == default(DateTime) ? string.Empty : d.ToString(DateTimeType);
        }

        private static string ToCommonDate(this DateTime d)
        {
            return defaultEmpty && d == default(DateTime) ? string.Empty : d.ToString(DateTimeType);
        }

        private static string ToCommonDateTime(this DateTime? d)
        {
            return d.HasValue ? d.Value.ToCommonDateTime() : string.Empty;
        }

        private static string ToCommonDate(this DateTime? d)
        {
            return d.HasValue ? d.Value.ToCommonDate() : string.Empty;
        }

        #endregion


    }
}
