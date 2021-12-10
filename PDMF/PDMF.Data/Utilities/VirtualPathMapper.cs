using System;
using System.IO;

namespace PDMF.Data.Utilities
{
    public static class VirtualPathMapper
    {
        private static bool IsInitialized { get; set; }
        
        public static void Initialize(string appRootPath)
        {
            if (IsInitialized)
            {
                throw new InvalidOperationException("Virtual path mapper is already initialized.");
            }

            _getAppRootPath = () => appRootPath;
            
            IsInitialized = true;
        }

        private static Func<string> _getAppRootPath;
        
        public static string MapToPhysicalPath(string virtualPath)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Virtual path mapper is not initialized.");
            }
            
            virtualPath = virtualPath.Replace("~/", string.Empty).Replace("~\\", string.Empty);
            return Path.Combine(_getAppRootPath(), virtualPath);
        }
    }
}