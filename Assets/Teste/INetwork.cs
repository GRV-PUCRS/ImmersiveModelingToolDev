using System;
using System.Threading.Tasks;

public interface INetwork
{
    Task SendFile(string filePath, Action completedCallback, Action<float> progressCallback, Action<string> errorCallback);
}
