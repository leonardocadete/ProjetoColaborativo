using System;

namespace ProjetoColaborativo.Business.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsClassOrSubclassOf<T>(this Type t)
        {
            return IsClassOrSubclassOf(t, typeof(T));
        }

        public static bool IsClassOrSubclassOf(Type t, Type typeToCompare)
        {
            if (t == null) throw new ArgumentNullException("t");
            if (t == typeToCompare) return true;
            if (t.BaseType == null) return false;
            if (t.BaseType == typeToCompare) return true;
            if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeToCompare) return true;

            return IsClassOrSubclassOf(t.BaseType, typeToCompare);
        }
    }
}
