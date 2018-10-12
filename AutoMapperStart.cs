using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;


    public static class AutoMapperStart
    {
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

        public static void CreateMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<DateTime, string>().ConvertUsing(d => d.ToString("yyyy-MM-dd HH:mm:ss"));
            cfg.CreateMap<string, DateTime>().ConvertUsing(ConvertToDateTime);
            cfg.CreateMap<string, DateTime?>().ConvertUsing(ConvertToDateTimeNullable);
        }

        public static readonly object TypeLock = new object();
        public static T MapTo<T>(this object source) where T : class
        {
            //if (source == null) throw new ArgumentNullException($"自动转换失败，类型{source.GetType().Name},{typeof(T).Name}");
            CreateMapper(source.GetType(), typeof(T));
            return Mapper.Map<T>(source);
        }

        public static T MapTo<T>(this object source, object dest) where T : class
        {
            CreateMapper(source.GetType(), typeof(T));
            return (T)Mapper.Map(source, dest, source.GetType(), typeof(T));
        }
        public static List<T> MapToList<T>(this IEnumerable<object> source) where T : class
        {
            if (source == null) return null;
            //if (!source.Any()) return new List<T>();
            //if (source == null) throw new ArgumentNullException($"自动转换失败，类型{source.GetType().Name},{typeof(T).Name}");
            var sourceType = source.GetType().GetGenericArguments().First();
            CreateMapper(sourceType, typeof(T));
            return Mapper.Map<List<T>>(source);
        }

        public static List<T> MapToList<T>(this IEnumerable<object> source, object info) where T : class
        {
            if (source == null) return null;
            if (!source.Any()) return new List<T>();
            var sourceType = source.GetType().GetGenericArguments().First();
            CreateMapper(sourceType, typeof(T));
            CreateMapper(sourceType, info.GetType());
            return source.Select(s =>
            {
                var d = Mapper.Map<T>(info);
                return (T)Mapper.Map(source, d, source.GetType(), typeof(T));
            }).ToList();
        }

        public static T Map<T>(params object[] sources) where T : class, new()
        {
            var dest = new T();
            sources.ToList().ForEach(s => dest = s.MapTo<T>(dest));
            return dest;
        }

        private static void CreateMapper(Type source, Type dest)
        {
            var typeMapper = Mapper.Instance.ConfigurationProvider.FindTypeMapFor(source, dest);
            if (typeMapper == null)
            {
                lock (TypeLock)
                {
                    typeMapper = Mapper.Instance.ConfigurationProvider.FindTypeMapFor(source, dest);
                    if (typeMapper != null) return;

                    var typeMappers = Mapper.Instance.ConfigurationProvider.GetAllTypeMaps();
                    Mapper.Reset();
                    Mapper.Initialize(cfg =>
                    {
                        typeMappers.ToList().ForEach(t => cfg.CreateMap(t.SourceType, t.DestinationType));
                        cfg.CreateMap(source, dest);
                    });
                }
            }
        }
    }

