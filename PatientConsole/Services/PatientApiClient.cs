using System.Net.Http.Json;
using PatientApiClient.Models;
using System.Net;

namespace PatientApiClient.Services
{
    public class PatientApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5240/api/Patient";

        public PatientApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PingApp()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/ping");
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        public async Task<List<Patient>> GetPatientsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Patient>>(BaseUrl);
        }

        public async Task<Patient> GetPatientAsync(int medicalRecordNumber)
        {
            return await _httpClient.GetFromJsonAsync<Patient>($"{BaseUrl}/{medicalRecordNumber}");
        }

        public async Task AddPatientAsync(Patient patient)
        {
            await _httpClient.PostAsJsonAsync(BaseUrl, patient);
        }

        public async Task<HttpResponseMessage> UpdatePatientAsync(int medicalRecordNumber, Patient patient)
        {
            return await _httpClient.PutAsJsonAsync($"{BaseUrl}/{medicalRecordNumber}",patient);
        }

        public async Task<HttpResponseMessage> DeletePatientAsync(int id)
        {
            return await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        }

    }
}
