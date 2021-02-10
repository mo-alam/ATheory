using System;
using System.IO;
using System.Text.Json;

namespace ATheory.Util.Extensions
{
    public class JsonShell<T> where T : class
    {
        #region Properties
        public string Error { get; private set; } 

        #endregion

        #region Public methods

        public T Load(string filename)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(filename));
            }
            catch (Exception e)
            {
                Error = e.ToMessage();
            }
            return null;
        }

        public bool Save(T instance, string filename)
        {
            try
            {
                File.WriteAllText(filename, JsonSerializer.Serialize(instance));
                return true;
            }
            catch (Exception e)
            {
                Error = e.ToMessage();
            }
            return false;
        }

        #endregion
    }
}
