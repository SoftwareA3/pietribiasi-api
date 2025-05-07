using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Services.Utils.Abstraction
{
    public interface ILogService
    {
        void AppendMessageToLog(string requestType, int? statusCode, string statusMessage, bool isActive);
        void AppendMessageAndListToLog<T>(string requestType, int? statusCode, string statusMessage, List<T> list, bool isActive);
        void AppendMessageAndItemToLog<T>(string requestType, int? statusCode, string statusMessage, T item, bool isActive);
    }
}