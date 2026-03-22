namespace Credenciamento.Application.Interfaces.Global;

public interface ICacheService
{
    /// <summary>
    /// Verifica se existe valor para uma determinada Chave
    /// </summary>
    /// <param name="key">Chave a ser consultada</param>
    /// <returns>Retorna se há valor para a chave</returns>
    bool HasKey(string key);

    /// <summary>
    /// Inserção de valor string no cache
    /// </summary>
    /// <param name="key">Chave de identificação do valor</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <returns>Retorna true se a operação for bem-sucedida, caso contrário, false</returns>
    bool SetString(string key, string value);

    /// <summary>
    /// Inserção de valor string no cache
    /// </summary>
    /// <param name="key">Chave de identificação do valor</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <param name="ltv">Tempo de vida do valor</param>
    /// <returns>Retorna true se a operação for bem-sucedida, caso contrário, false</returns>
    bool SetString(string key, string value, int ltv);

    /// <summary>
    /// Leitura de valor contido em uma determinada chave
    /// </summary>
    /// <param name="key">Chave de consulta</param>
    /// <returns>Retorna o valor associado à chave, ou uma string vazia se a chave não existir</returns>
    string GetString(string key);

    /// <summary>
    /// Inserção de object no cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor a ser armazenado</typeparam>
    /// <param name="key">Chave de identificação do valor</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <returns>Retorna true se a operação for bem-sucedida, caso contrário, false</returns>
    bool SetObject<T>(string key, T value);

    /// <summary>
    /// Inserção de object no cache
    /// </summary>
    /// <typeparam name="T">Tipo do valor a ser armazenado</typeparam>
    /// <param name="key">Chave de identificação do valor</param>
    /// <param name="value">Valor a ser armazenado</param>
    /// <param name="ltv">Tempo de vida do valor</param>
    /// <returns>Retorna true se a operação for bem-sucedida, caso contrário, false</returns>
    bool SetObject<T>(string key, T value, int ltv);

    /// <summary>
    /// Leitura de object contido em uma determinada chave
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T GetObject<T>(string key);

    /// <summary>
    /// Lista todas as chaves atualmente armazenadas no cache (útil para diagnóstico e depuração)
    /// </summary>
    /// <returns>Lista com todas as chaves</returns>
    IEnumerable<string> GetAllCacheKeys();

    /// <summary>
    /// Remove uma chave específica do cache, eliminando o valor associado a ela. 
    /// </summary>
    /// <param name="key">Chave a ser removida</param>
    /// <returns>Retorna true se a remoção for bem-sucedida, ou false caso contrário</returns>
    bool RemoveKey(string key);

    /// <summary>
    /// Retorna um snapshot do cache, incluindo todas as chaves e seus valores associados.
    /// </summary>
    /// <returns>Dicionário contendo todas as chaves e seus valores</returns>
    Dictionary<string, object> GetCacheSnapshot();
}
