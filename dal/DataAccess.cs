﻿using Sleave.connection;
using Sleave.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sleave.dal
{
    class DataAccess
    {
        /// <summary>
        /// Chaine de connexion à la base de données
        /// </summary>
        private static string connectionString = "server=localhost;user id=responsable;password=pwd;database=mediatek86;SslMode=none";

        /// <summary>
        /// Controle si l'utilisateur a le droit de se connecter
        /// </summary>
        /// <param name="login"></param>
        /// <param name="pwd"></param>
        /// <returns>Vrai ou Faux</returns>
        public static Boolean ControlAccess(string login, string pwd)
        {
            string req = "SELECT * FROM responsable res ";
            req += "WHERE res.login=@login AND pwd=SHA2(@pwd, 256);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@login", login);
            parameters.Add("@pwd", pwd);
            ConnectionDataBase curs = ConnectionDataBase.GetInstance(connectionString);
            curs.ReqSelect(req, parameters);
            if (curs.Read())
            {
                curs.Close();
                return true;
            }
            else
            {
                curs.Close();
                return false;
            }
        }

        /// <summary>
        /// Récupère et retourne les personnels de la base de données
        /// </summary>
        /// <returns>Liste des personnels</returns>
        public static List<Personnel> GetPersonnel()
        {
            List<Personnel> personnel = new List<Personnel>();
            string req = "SELECT p.idpersonnel AS idPersonnel, s.idservice AS idDept, s.nom AS dept, p.nom AS lastName, p.prenom AS firstName, p.tel AS phone, p.mail AS mail ";
            req += "FROM personnel p ";
            req += "JOIN service AS s ON (p.idservice = s.idservice) ";
            req += "ORDER BY p.idpersonnel ";
            ConnectionDataBase curs = ConnectionDataBase.GetInstance(connectionString);
            curs.ReqSelect(req, null);
            while (curs.Read())
            {
                Personnel pers = new Personnel((int)curs.Field("idPersonnel"), (string)curs.Field("lastName"), (string)curs.Field("firstName"), (string)curs.Field("phone"), (string)curs.Field("mail"), (int)curs.Field("idDept"), (string)curs.Field("dept"));
                personnel.Add(pers);
            }
            curs.Close();
            return personnel;
        }

        /// <summary>
        /// Récupère et retourne les services de la base de données
        /// </summary>
        /// <returns>Liste des services</returns>
        public static List<Dept> GetDepts()
        {
            List<Dept> depts = new List<Dept>();
            string req = "SELECT idservice AS idDept, nom AS dept ";
            req += "FROM `service` ";
            req += "WHERE 1 ";
            req += "ORDER BY dept ";
            ConnectionDataBase curs = ConnectionDataBase.GetInstance(connectionString);
            curs.ReqSelect(req, null);
            while (curs.Read())
            {
                Dept dept = new Dept((int)curs.Field("idDept"), (string)curs.Field("dept"));
                depts.Add(dept);
            }
            curs.Close();
            return depts;
        }
    }
}
