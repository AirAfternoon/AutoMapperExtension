
using AutoMapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMapperExtensions
{
    public static class AutoMapperExtensions
    {
        public static IMapper _mapper = null;
        public static readonly object TypeLock = new object();
        public static T MapTo<T>(this object source) where T : class
        {
            CreateMapper(source.GetType(), typeof(T));
            return _mapper.Map<T>(source);
        }

        public static T MapTo<T>(this object source, object dest) where T : class
        {
            CreateMapper(source.GetType(), typeof(T));
            return (T)_mapper.Map(source, dest, source.GetType(), typeof(T));
        }
        public static List<T> MapToList<T>(this IEnumerable<object> source) where T : class
        {
            if (source == null) return null;
            if (!source.Any()) return new List<T>();
            var sourceType = source.GetType().GetGenericArguments().First();
            CreateMapper(sourceType, typeof(T));
            return _mapper.Map<List<T>>(source);
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
                var d = _mapper.Map<T>(info);
                return (T)_mapper.Map(source, d, source.GetType(), typeof(T));
            }).ToList();
        }

        private static void CreateMapper(Type source, Type dest)
        {
            var typeMapper = _mapper.ConfigurationProvider.FindTypeMapFor(source, dest);
            if (typeMapper != null) return;
            lock (TypeLock)
            {
                typeMapper = _mapper.ConfigurationProvider.FindTypeMapFor(source, dest);
                if (typeMapper != null) return;

                AutoMapperConfig.express.CreateMap(source, dest);
                _mapper = new MapperConfiguration(AutoMapperConfig.express).CreateMapper();
            }
        }

        public static T Map<T>(object obj)
        {
            if (obj != null)
                CreateMapper(obj.GetType(), typeof(T));
            return _mapper.Map<T>(obj);
        }

        public static List<T> MapList<T>(IEnumerable<object> list)
        {
            CreateMapper(list.GetType().GetGenericArguments().First(), typeof(T));
            return _mapper.Map<List<T>>(list);
        }

        public static T Map<T>(params object[] sources) where T : class, new()
        {
            var dest = new T();
            sources.ToList().ForEach(s =>
            {
                if (s != null)
                    dest = s.MapTo<T>(dest);
            });
            return dest;
        }

        public static TDest Map<TSource, TDest>(object obj)
        {
            CreateMapper(typeof(TSource), typeof(TDest));
            return _mapper.Map<TDest>(obj);
        }
    }
}
