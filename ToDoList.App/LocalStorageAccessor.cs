
using System.Xml.Serialization;
using Microsoft.JSInterop;

using System.Text.Json;

namespace ToDoList.App {
    public class LocalStorageAccessor : IAsyncDisposable
    {   
        private Lazy<IJSObjectReference> _accessorJsRef = new ();
        private readonly IJSRuntime _JsRuntime;

        public LocalStorageAccessor(IJSRuntime jsRuntime)
        {
            _JsRuntime = jsRuntime;
        }

        public async Task<T> GetValueAsync<T>(string key) 
        {
            await WaitForReference();
            var jsonResult = await _accessorJsRef.Value.InvokeAsync<string>("get", key);
            var result = JsonSerializer.Deserialize<T>(jsonResult);
            return result;
        }

        public async Task SetValueAsync<T>(string key, T value) 
        {
            await WaitForReference();
            string jsonValue = JsonSerializer.Serialize(value);
            await _accessorJsRef.Value.InvokeVoidAsync("set", key, jsonValue);
        }

        public async Task RemoveValueAsync(string key) 
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("remove", key);
        }

        public async Task ClearAsync() 
        {
            await WaitForReference();
            await _accessorJsRef.Value.InvokeVoidAsync("clear");
        }

        private async Task WaitForReference()
        {
            if (!_accessorJsRef.IsValueCreated)
            {
                _accessorJsRef = new(await _JsRuntime.InvokeAsync<IJSObjectReference>
                ("import", "/js/LocalStorageAccessor.js"));

            }
        }
        public async ValueTask DisposeAsync()
        {
            if (_accessorJsRef.IsValueCreated)
            {
                await _accessorJsRef.Value.DisposeAsync();
            }
        }
    }
}