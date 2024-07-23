using System;
using System.Collections.Generic;
using System.Linq;

using Dapper;

using System.Data.SqlClient;
using System.IO;

namespace UpdateHosts
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "server=192.168.0.89; user id = sa; pwd = xxxxxxx; database = XXXXXXXX";
            string query = "SELECT HostName, IPAddress FROM IT_T_HOSTS";

            using (var connection = new SqlConnection(connectionString))
            {
                var hostsEntries = connection.Query<HostsEntry>(query);

                foreach (var entry in hostsEntries)
                {
                    Console.WriteLine($"HostName: {entry.HostName}, IPAddress: {entry.IPAddress}");
                }

                UpdateHostsFile(hostsEntries);
            }
        }

        static void UpdateHostsFile(IEnumerable<HostsEntry> hostsEntries)
        {
            string hostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";

            if (!File.Exists(hostsFilePath))
            {
                Console.WriteLine("Hosts file does not exist!");
                return;
            }

            var existingEntries = File.ReadAllLines(hostsFilePath);
            var updatedEntries = new List<string>(existingEntries);

            foreach (var entry in hostsEntries)
            {
                string newEntry = $"{entry.IPAddress} {entry.HostName}";

                // Check if the entry already exists
                bool entryExists = existingEntries.Any(line => line.Contains(entry.HostName));

                if (!entryExists)
                {
                    updatedEntries.Add(newEntry);
                }
                else
                {
                    // Update the existing entry
                    for (int i = 0; i < updatedEntries.Count; i++)
                    {
                        if (updatedEntries[i].Contains(entry.HostName))
                        {
                            updatedEntries[i] = newEntry;
                            break;
                        }
                    }
                }
            }

            File.WriteAllLines(hostsFilePath, updatedEntries);
            Console.WriteLine("Hosts file updated successfully!");
        }
    }

    class HostsEntry
    {
        public string HostName { get; set; }
        public string IPAddress { get; set; }
    }
}
