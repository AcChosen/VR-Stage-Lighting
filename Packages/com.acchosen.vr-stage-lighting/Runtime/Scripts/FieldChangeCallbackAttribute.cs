#if !UDONSHARP

using System;

namespace VRSL
{
    internal class FieldChangeCallbackAttribute : Attribute
    {
        public FieldChangeCallbackAttribute(string v)
        {
            V = v;
        }

        public string V { get; }
    }
}
#else
namespace VRSL
{
}
#endif