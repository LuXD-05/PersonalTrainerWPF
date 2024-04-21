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
        /// Adds an user to the xml archive
        /// </summary>
        /// <param name="u">User to add</param>
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
        /// Gets user data from the xml archive
        /// </summary>
        /// <returns>List of User</returns>
        public static List<User> GetUsers()
        {
            var l = new List<User>();

            try
            {
                // If file exists
                if (File.Exists(users_path))
                {
                    // Reads it
                    var xmls = new XmlSerializer(typeof(List<User>));
                    var sr = new StreamReader(users_path);
                    l = (List<User>)xmls.Deserialize(sr);
                    sr.Close();
                }
                else
                {
                    // Creates it
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
        /// Updates users list in the xml archive
        /// </summary>
        /// <param name="l">Users to update</param>
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
