using Credenciamento.Application.Interfaces.Global;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Credenciamento.Application.Services.Glogal;

public class CacheService : ICacheService
{
    private readonly ILogger _logger;
    private static IMemoryCache _memoryCache;
    private static readonly HashSet<string> _cacheKeys = new HashSet<string>();
    private static readonly object _lockObject = new object();

    private readonly JsonSerializerOptions defaultJsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = false,
        MaxDepth = 30,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        ReferenceHandler = ReferenceHandler.Preserve
    };
    private const int defaultLtv = 60;

    public CacheService(ILogger<CacheService> logger)
    {
        _logger = logger;
    }

    public bool HasKey(string key)
    {
        var cache = GetCache();
        return cache.TryGetValue(key, out string value);
    }

    #region String Values
    public bool SetString(string key, string value)
    {
        return SetString(key, value, defaultLtv);
    }
    public bool SetString(string key, string value, int ltv)
    {
        try
        {
            var cache = GetCache();
            cache.Set(key, value, DateTimeOffset.UtcNow.AddMinutes(ltv));

            lock (_lockObject)
            {
                _cacheKeys.Add(key);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetString - {0}", ex.Message);
            return false;
        }
    }
    public string GetString(string key)
    {
        string returns = "";

        try
        {
            var cache = GetCache();
            if (cache is not null && cache.TryGetValue(key, out string value))
                return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetString {0}", ex.Message);
        }

        return returns;
    }
    #endregion

    #region Object Values
    public bool SetObject<T>(string key, T value)
    {
        return this.SetObject(key, value, defaultLtv);
    }
    public bool SetObject<T>(string key, T value, int ltv)
    {
        try
        {
            string content = JsonSerializer.Serialize(value, defaultJsonSerializerOptions);
            return this.SetString(key, content, ltv);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SetObject {0}", ex.Message);
            return false;
        }
    }
    public T GetObject<T>(string key)
    {
        T returns = (T)Activator.CreateInstance(typeof(T));

        try
        {
            returns = JsonSerializer.Deserialize<T>(this.GetString(key), defaultJsonSerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetObject {0}", ex.Message);
        }

        return returns;
    }
    #endregion

    #region Diagnostic Methods
    /// <summary>
    /// Obtém todas as chaves armazenadas no cache (útil para debug)
    /// </summary>
    public IEnumerable<string> GetAllCacheKeys()
    {
        lock (_lockObject)
        {
            return _cacheKeys.ToList();
        }
    }

    /// <summary>
    /// Remove uma chave específica do cache
    /// </summary>
    public bool RemoveKey(string key)
    {
        try
        {
            var cache = GetCache();
            cache.Remove(key);

            lock (_lockObject)
            {
                _cacheKeys.Remove(key);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Remove - {0}", ex.Message);
            return false;
        }
    }
    #endregion

    #region Diagnostic Methods
    /// <summary>
    /// Retorna informações detalhadas sobre o cache (inclui valores)
    /// </summary>
    public Dictionary<string, object> GetCacheSnapshot()
    {
        var snapshot = new Dictionary<string, object>();

        foreach (var key in _cacheKeys)
        {
            try
            {
                var value = GetString(key);
                snapshot[key] = value ?? "<null>";
            }
            catch
            {
                snapshot[key] = "<erro ao ler>";
            }
        }

        return snapshot;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Obtém todas as chaves do cache usando reflection (APENAS PARA DEBUG)
    /// </summary>
    private IEnumerable<string> GetAllCacheKeysViaReflection()
    {
        try
        {
            var cache = GetCache();
            var coherentState = cache.GetType()
                .GetField("_coherentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(cache);

            var entries = coherentState?.GetType()
                .GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(coherentState);

            if (entries is System.Collections.IDictionary dictionary)
            {
                var keys = new List<string>();
                foreach (var key in dictionary.Keys)
                {
                    keys.Add(key.ToString());
                }
                return keys;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAllCacheKeysViaReflection - {0}", ex.Message);
        }

        return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Instancia o cache se ainda não existir (singleton simples)
    /// </summary>
    /// <returns></returns>
    private static IMemoryCache GetCache()
    {
        if (null == _memoryCache)
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        return _memoryCache;
    }
    #endregion
}


