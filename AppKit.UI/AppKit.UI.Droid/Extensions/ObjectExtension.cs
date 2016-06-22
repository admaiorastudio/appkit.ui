namespace AdMaiora.AppKit.UI
{
    using System;

    internal class JavaHolder : Java.Lang.Object
    {
        public readonly object Instance;

        public JavaHolder(object instance)
        {
            this.Instance = instance;
        }

        public override string ToString()
        {
            return this.Instance.ToString();
        }
    }

    public static class ObjectExtension
    {
        public static TObject ToNetObject<TObject>(this Java.Lang.Object value)
        {
            if (value == null)
                return default(TObject);

            if (!(value is JavaHolder))
                throw new InvalidOperationException("Unable to convert to .NET object. Only Java.Lang.Object created with .ToJavaObject() can be converted.");

            return
                (TObject)((JavaHolder)value).Instance;
        }

        public static Java.Lang.Object ToJavaObject<TObject>(this TObject value)
        {
            if (value == null)
                return null;

            var holder = new JavaHolder(value);

            return
                (Java.Lang.Object)holder;
        }
    }
}