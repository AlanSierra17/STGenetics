namespace STGeneticsTest.Models
{
    public class DefaultResponse<T>
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public T? Data { get; set; }

        public DefaultResponse()
        {
            Error = false;
            Msg = string.Empty;
            Data = default(T);
        }
    }
}
