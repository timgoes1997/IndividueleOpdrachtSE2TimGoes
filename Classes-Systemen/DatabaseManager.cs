﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.DataAccess.Client;

namespace IndividueleOpdrachtSE2
{
    public class DatabaseManager
    {
        public string PCN;
        public string Wachtwoord;
        public OracleConnection Verbinding;

        public DatabaseManager()
        {
            this.PCN = "dbi322878";
            this.Wachtwoord = "YB4G0mfwlX";

            Verbinding = new OracleConnection();

            Verbinding.ConnectionString = "User Id=" + this.PCN + ";Password=" + this.Wachtwoord + ";Data Source=" + "//192.168.15.50:1521/fhictora;";
        }

        /// <summary>
        /// Retourneert een instantie van OracleCommand met
        /// this.Verbinding & .CommandType.Text
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public OracleCommand MaakOracleCommand(string sql)
        {
            OracleCommand command = new OracleCommand(sql, this.Verbinding);
            command.CommandType = System.Data.CommandType.Text;

            return command;
        }

        /// <summary>
        /// Voert de query uit van meegegeven OracleCommand.
        /// Deze OracleCommand moet gemaakt zijn door MaakOracleCommand() en parameters moeten al ingesteld zijn.
        /// De teruggegeven lijst bevat voor elke rij een OracleDataReader.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public OracleDataReader VoerMultiQueryUit(OracleCommand command)
        {
            try
            {
                Verbinding.Open();

                OracleDataReader reader = command.ExecuteReader();

                return reader;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Voert de query uit van meegegeven OracleCommand.
        /// Deze OracleCommand moet gemaakt zijn door MaakOracleCommand() en parameters moeten al ingesteld zijn.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public OracleDataReader VoerQueryUit(OracleCommand command)
        {
            try
            {
                if (Verbinding.State == ConnectionState.Closed)
                {
                    Verbinding.Open();
                }

                OracleDataReader reader = command.ExecuteReader();

                reader.Read();

                return reader;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Voert een Delete, Update of Insert query uit op de database.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool VoerNonQueryUit(OracleCommand command)
        {
            try
            {
                if (Verbinding.State == ConnectionState.Closed)
                {
                    Verbinding.Open();
                }

                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Registreert een gebruiker in de database.
        /// </summary>
        /// <param name="naam"></param>
        /// <param name="wachtwoord"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool RegistreerGebruiker(string naam, string wachtwoord, string email)
        {
            try
            {
                string sql = "INSERT INTO GEBRUIKER (ID, GEBRUIKERSNAAM, WACHTWOORD, EMAIL, REGISTRATIEDATUM) VALUES (:ID, :GEBRUIKERSNAAM, :WACHTWOORD, :EMAIL, :REGISTRATIEDATUM)";

                OracleCommand command = MaakOracleCommand(sql);

                command.Parameters.Add(":ID", VerkrijgNieuwGebruikerID());
                command.Parameters.Add(":GEBRUIKERSNAAM", naam);
                command.Parameters.Add(":WACHTWOORD", wachtwoord);
                command.Parameters.Add(":EMAIL", email);
                command.Parameters.Add(":REGISTRATIEDATUM", DateTime.Now);

                VoerNonQueryUit(command);
                
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                Verbinding.Close();
            }
        }

        /// <summary>
        /// Verkrijgt een nieuw gebruikers ID, zodat een nieuwe gebruiker in de database kan worden geplaatst.
        /// </summary>
        /// <returns></returns>
        public int VerkrijgNieuwGebruikerID()
        {
            try
            {
                string sql = "SELECT MAX(ID) AS ID FROM GEBRUIKER";

                OracleCommand command = MaakOracleCommand(sql);

                string text = command.CommandText;

                OracleDataReader reader = VoerQueryUit(command);

                return Convert.ToInt32(reader["ID"].ToString()) + 1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                Verbinding.Close();
            }
        }

        public string VerkrijgWachtwoord(string gebruikersnaam)
        {
            try
            {
                string sql = "SELECT WACHTWOORD FROM GEBRUIKER WHERE GEBRUIKERSNAAM = :GEBRUIKERSNAAM";

                OracleCommand command = MaakOracleCommand(sql);

                command.Parameters.Add(":GEBRUIKERSNAAM", gebruikersnaam);

                string text = command.CommandText;

                OracleDataReader reader = VoerQueryUit(command);

                return reader["WACHTWOORD"].ToString();
            }
            catch
            {
                return null;
            }
            finally
            {
                Verbinding.Close();
            }
        }
    }
}