using System;
using System.Reflection;

namespace MessageService.Core.Util
{
    public static class AssistClass
    {
        public static void ExceptionWhenNull(object o)
        {
            if(IsNull(o))
            {
                throw new NullReferenceException("object can't be null or empty");
            }
        }

        public static bool IsNull(object o)
        {
            return o == null || o.Equals(null) || ReferenceEquals(o, null);
        }
        public static class StringAssist
        {

            public static  bool IsNullOrEmpty(string str)
            {
                return string.IsNullOrEmpty(str);
            }
             public static  void ExceptionWhenStringEmpty(string str)
            {
                ExceptionWhenNull(str);
                if ( string.IsNullOrEmpty(str))
                {
                    throw new ApplicationException("sting can not allowed empty value");
                }
            }
        }

        public static class ArrayAssist
        {
            public static bool HasElements( Array array)
            {
                return array.Length > 0;
            }
        }

        public static class AssemblyAssist
        {
            static Assembly[] _cache;
             public static Assembly[] CurrentDomainAssemblies
            {
                get
                {
                    if (_cache == null || _cache.Length == 0)
                    {
                        _cache = AppDomain.CurrentDomain.GetAssemblies();
                    }
                    return _cache;
                }
                set
                {
                    _cache = value;
                }
            }
        }

    }
}
