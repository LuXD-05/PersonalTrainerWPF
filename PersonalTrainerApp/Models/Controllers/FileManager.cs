using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Windows;

namespace PersonalTrainerApp.Models.Controllers
{
    public static class FileManager
    {
        static string users_path = Directory.GetCurrentDirectory() + "\\users.xml";

        #region Public Methods

        /// <summary>
        /// Aggiunge un utente all'archivio xml
        /// </summary>
        /// <param name="u">Utente da aggiungere</param>
        public static void AddUser(User u)
        {
            try
            {
                var xmls = new XmlSerializer(typeof(User));
                var sw = new StreamWriter(users_path, true);
                xmls.Serialize(sw, u);
                sw.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show("Err AddUser");
            }
        }

        /// <summary>
        /// Ottiene i gli utenti dall'archivio xml
        /// </summary>
        /// <returns>Lista di utenti</returns>
        public static List<User> GetUsers()
        {
            var l = new List<User>();

            try
            {
                // Se il file esiste
                if (File.Exists(users_path))
                {
                    // Lo leggo
                    var xmls = new XmlSerializer(typeof(List<User>));
                    var sr = new StreamReader(users_path);
                    l = (List<User>)xmls.Deserialize(sr);
                    sr.Close();
                }
                else
                {
                    // Se no lo creo
                    var xmls = new XmlSerializer(typeof(List<User>));
                    var sw = new StreamWriter(users_path, false);
                    xmls.Serialize(sw, l);
                    sw.Close();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Err GetUsers");
            }

            return l;
        }

        /// <summary>
        /// Aggiorna la lista di utenti nell'archivio xml
        /// </summary>
        /// <param name="l">Utenti da aggiornare</param>
        public static void UpdateDb(List<User> l)
        {
            try
            {
                var xmls = new XmlSerializer(typeof(List<User>));
                var sw = new StreamWriter(users_path, false);
                xmls.Serialize(sw, l);
                sw.Close();
            }
            catch (Exception)
            {
                //MessageBox.Show("Err UpdateDb");
            }
        }

        #endregion
    }
}
