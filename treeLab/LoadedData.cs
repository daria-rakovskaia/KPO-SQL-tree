using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace treeLab
{
    class LoadedData : IComparable<LoadedData>
    {
        static string connectionString = @"Data Source=.\DEV;" +
                                         "Initial Catalog=Procedures;" +
                                         "Integrated Security=true;";

        public int CountryId { get; set; }

        public string Country { get; set; }

        public int MakerId { get; set; }

        public string Maker { get; set; }

        public int VaccineId { get; set; }

        public string Vaccine { get; set; }

        public LoadedData(int countryId, string country, int makerId, string maker, int vaccineId, string vaccine)
        {
            CountryId = countryId;
            Country = country;
            MakerId = makerId;
            Maker = maker;
            VaccineId = vaccineId;
            Vaccine = vaccine;
        }

        public static IEnumerable<LoadedData> GetData()
        {
            var data = new List<LoadedData>();
            string query = "select Countries.Id as 'C_Id', Countries.Name as 'Country', " +
                           "VaccinesMakers.Id as 'Vm_Id', VaccinesMakers.Name as 'Maker', " +
                           "Vaccines.Id as 'V_Id', Vaccines.Name as 'Vaccine' from VaccinesMakers " +
                           "join Countries on Countries.id = VaccinesMakers.CountryId " +
                           "join Vaccines on Vaccines.MakerId = VaccinesMakers.id;";
            using (var connect = new SqlConnection(connectionString))
            {
                connect.Open();
                var cmd = new SqlCommand(query, connect);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data.Add(new LoadedData(reader.GetInt32(0), reader.GetString(1).Trim(),
                                            reader.GetInt32(2), reader.GetString(3).Trim(),
                                            reader.GetInt32(4), reader.GetString(5).Trim()));
                }
                reader.Close();
                connect.Close();
            }
            return data;
        }

        public int CompareTo(LoadedData other)
        {
            return other.CountryId.CompareTo(CountryId);
        }
    }
}
