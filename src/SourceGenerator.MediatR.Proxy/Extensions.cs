namespace SourceGenerator.MediatR.Proxy
{
    internal static class Extensions
    {
        public static string StripPostfix(this string requestName, string postfixToStrip)
        {
            int postfixIndex = requestName.LastIndexOf(postfixToStrip);
            return postfixIndex > 0
                ? requestName.Substring(0, postfixIndex)
                : requestName;
        }
    }
}
