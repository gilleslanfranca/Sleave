﻿using Sleave.control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sleave.view
{
    /// <summary>
    /// Interface graphique de la connexion
    /// </summary>
    public partial class FrmConnection : Form
    {
        /// <summary>
        /// Instance du contrôleur
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Constructeur : Initialise les éléments de l'interface de connexion
        /// </summary>
        /// <param name="controller">Contrôleur</param>
        public FrmConnection(Controller controller)
        {
            this.controller = controller;
            InitializeComponent();

        }

        /// <summary>
        /// Vérifie que les champs sont remplis et demande l'accès à la BDD au contrôleur
        /// </summary>
        /// <param name="sender">L'objet concerné</param>
        /// <param name="e">L'évènement déclancheur</param>
        private void BtnConnection_Click(object sender, EventArgs e)
        {
            if (!txtLogin.Text.Equals("") && !txtPwd.Text.Equals(""))
            {
                if (!controller.ControlAccess(txtLogin.Text, txtPwd.Text))
                {
                    MessageBox.Show("Identifiant inconnu. Connexion échouée.", "Identification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtLogin.Text = "";
                    txtPwd.Text = "";
                    txtLogin.Focus();
                }
            }
        }
    }
}
