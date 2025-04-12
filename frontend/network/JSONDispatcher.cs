using System.Collections.Concurrent;
using ConsoleApp1.utils;
using Newtonsoft.Json.Linq;

namespace frontend.network;

public class JSONDispatcher
{
    private readonly ConcurrentDictionary<int, TaskCompletionSource<JObject>> _pendingRequests = new();
    private readonly ConcurrentDictionary<string, List<Action<JObject>>> _eventListeners = new();

    public void Dispatch(JObject message)
    {
        if (message.ContainsKey("messageId"))
        {
            
            int id = message["messageId"]!.ToObject<int>();
            if (_pendingRequests.TryRemove(id, out var tcs))
            {
                tcs.SetResult(message); 
            }
        }
        else
        {
            NotifyListeners(message); 
        }
    }

    private void NotifyListeners(JObject message)
    {
        string? eventType = message["eventType"]!.ToObject<string>();
        if (!_eventListeners.ContainsKey(eventType))
        {
            return;
        } 
        
        if (_eventListeners.TryGetValue(eventType, out var listeners))
        {
            foreach (var listener in listeners)
            {
                listener.Invoke(message); 
            }
        }
    }

    public TaskCompletionSource<JObject> AddPendingRequest(JObject request)
    {
       TaskCompletionSource<JObject> tcs = new();
       int id = request["messageId"]!.ToObject<int>();
       _pendingRequests.TryAdd(id, tcs);
       return tcs;  
    }
    
    public void OnEvent(String eventType, Action<JObject> listener)
    {
        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = new List<Action<JObject>>();
        }
        
        _eventListeners[eventType].Add(listener);
    }
}