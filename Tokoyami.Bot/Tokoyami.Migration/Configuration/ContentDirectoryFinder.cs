using System;
using System.IO;

namespace Tokoyami.Context.Configuration
{
    public static class ContentDirectoryFinder
    {
        public static string CalculateContentRootFolder()
        {
            return Path.Combine(AppContext.BaseDirectory, string.Format("..{0}..{0}..{0}", Path.DirectorySeparatorChar));
        }
    }
}
