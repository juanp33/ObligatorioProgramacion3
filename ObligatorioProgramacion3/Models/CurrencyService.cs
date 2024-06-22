namespace ObligatorioProgramacion3.Models
{
    

    using RestSharp;
    using System.Threading.Tasks;

    public class CurrencyService
    {
        private readonly RestClient _client;

        public CurrencyService()
        {
            _client = new RestClient("http://api.currencylayer.com");
        }

        public async Task<string> GetExchangeRateAsync()
        {
            var client = new RestClient("http://api.currencylayer.com");

            var request = new RestRequest("live", Method.Get);
            request.AddParameter("access_key", "b6fb3ee2a8859b4237975e1d708cb64e");
            request.AddParameter("currencies", "UYU");

            var response = await client.ExecuteAsync(request);
            return (response.Content);

        }
    }

}