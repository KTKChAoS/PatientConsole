using PatientApiClient.Models;

namespace PatientApiClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            var apiClient = new Services.PatientApiClient(httpClient);

            while (true)
            {
                string response = await PingApp(apiClient);
                if (response == "Fail")
                {
                    Console.WriteLine("Service offline, trying again in 5 seconds.");
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.WriteLine("Service online, continuing.");
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Add Patient");
                Console.WriteLine("2. Get Patient by ID");
                Console.WriteLine("3. Update Patient");
                Console.WriteLine("4. Delete Patient");
                Console.WriteLine("5. Get All Patients");
                Console.WriteLine("6. Exit");
                Console.Write("Enter number: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await AddPatient(apiClient);
                        break;
                    case "2":
                        await GetPatientById(apiClient);
                        break;
                    case "3":
                        await UpdatePatient(apiClient);
                        break;
                    case "4":
                        await DeletePatient(apiClient);
                        break;
                    case "5":
                        await GetAllPatients(apiClient);
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static async Task<String> PingApp(Services.PatientApiClient apiClient)
        {
            var response = await apiClient.PingApp();
            if (!response.IsSuccessStatusCode)
            {
                return "Fail";
            }
            else
            {
                return "Pass";
            }
            
        }

        static async Task AddPatient(Services.PatientApiClient apiClient)
        {
            var patient = new Patient();

            Console.Write("Enter Name: ");
            var name = Console.ReadLine();
            patient.Name = string.IsNullOrEmpty(name) ? null : name;

            Console.Write("Enter Age: ");
            var age = Console.ReadLine();
            while (string.IsNullOrEmpty(age))
            {
                Console.Write("Age is required. Enter Age: ");
                age = Console.ReadLine();
            }
            patient.Age = int.Parse(age);

            Console.Write("Enter Gender: ");
            var gender = Console.ReadLine();
            while (string.IsNullOrEmpty(gender))
            {
                Console.Write("Gender is required. Enter Gender: ");
                gender = Console.ReadLine();
            }
            patient.Gender = gender;

            Console.Write("Enter Contacts (comma separated): ");
            patient.Contacts = Console.ReadLine().Split(',');

            Console.Write("Enter Admitting Diagnosis: ");
            var diagnosis = Console.ReadLine();
            patient.AdmittingDiagnosis = string.IsNullOrEmpty(diagnosis) ? null : diagnosis;

            Console.Write("Enter Attending Physician: ");
            patient.AttendingPhysician = Console.ReadLine();

            Console.Write("Enter Department: ");
            patient.Department = Console.ReadLine();

            await apiClient.AddPatientAsync(patient);
            Console.WriteLine("Patient added successfully.");
        }

        static async Task GetPatientById(Services.PatientApiClient apiClient)
        {
            Console.Write("Enter Medical Record Number: ");
            var id = int.Parse(Console.ReadLine());

            var patient = await apiClient.GetPatientAsync(id);
            if (patient != null)
            {
                Console.WriteLine($"ID: {patient.MedicalRecordNumber}");
                Console.WriteLine($"Name: {patient.Name}");
                Console.WriteLine($"Age: {patient.Age}");
                Console.WriteLine($"Gender: {patient.Gender}");
                if (patient.Contacts == null)
                {
                    patient.Contacts = ["No contact found"];
                }
                Console.WriteLine($"Contacts: {string.Join(", ", patient.Contacts)}");
                Console.WriteLine($"Admitting Diagnosis: {patient.AdmittingDiagnosis}");
                Console.WriteLine($"Attending Physician: {patient.AttendingPhysician}");
                Console.WriteLine($"Department: {patient.Department}");
            }
            else
            {
                Console.WriteLine("Patient not found.");
            }
        }

        static async Task UpdatePatient(Services.PatientApiClient apiClient)
        {
            Console.Write("Enter Medical Record Number: ");
            var id = int.Parse(Console.ReadLine());
            Patient patient;
            try
            {
                patient = await apiClient.GetPatientAsync(id);
                if (patient == null)
                {
                    Console.WriteLine("Patient not found.");
                    return;
                }
                Console.Write("Enter Name: ");
                patient.Name = Console.ReadLine();

                Console.Write("Enter Age: ");
                patient.Age = int.Parse(Console.ReadLine());

                Console.Write("Enter Gender: ");
                patient.Gender = Console.ReadLine();

                Console.Write("Enter Contacts (comma separated): ");
                patient.Contacts = Console.ReadLine().Split(',');

                Console.Write("Enter Admitting Diagnosis: ");
                var diagnosis = Console.ReadLine();
                patient.AdmittingDiagnosis = string.IsNullOrEmpty(diagnosis) ? null : diagnosis;

                Console.Write("Enter Attending Physician: ");
                patient.AttendingPhysician = Console.ReadLine();

                Console.Write("Enter Department: ");
                patient.Department = Console.ReadLine();


                try
                {
                    var response = await apiClient.UpdatePatientAsync(id, patient);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Patient updated successfully.");
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        static async Task DeletePatient(Services.PatientApiClient apiClient)
        {
            Console.Write("Enter Medical Record Number: ");
            var id = int.Parse(Console.ReadLine());

            try
            {
                var response = await apiClient.DeletePatientAsync(id);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Patient deleted successfully.");
                }
                else
                {
                    Console.WriteLine(response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task GetAllPatients(Services.PatientApiClient apiClient)
        {
            var patients = await apiClient.GetPatientsAsync();
            if (patients.Count > 0)
            {
                Console.WriteLine("{0,-10} {1,-20} {2,-25} {3,-20}", "ID", "Name", "Diagnosis", "Physician");
                Console.WriteLine(new string('-', 75));
                foreach (var patient in patients)
                {
                    Console.WriteLine("{0,-10} {1,-20} {2,-25} {3,-20}",
                                      patient.MedicalRecordNumber,
                                      patient.Name,
                                      patient.AdmittingDiagnosis,
                                      patient.AttendingPhysician);
                }
            }
            else
            {
                Console.WriteLine("No patients found.");
            }
        }
    }
}
