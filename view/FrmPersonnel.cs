﻿using Sleave.control;
using Sleave.model;
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
    /// Interface graphique pour la gestion du personnel
    /// </summary>
    public partial class FrmPersonnel : Form
    {
        /// <summary>
        /// Constante : Unité de largeur des champs de la grille de données
        /// </summary>
        private const int fieldWidthUnit = 25;

        /// <summary>
        /// Constante : Nombre maximal de ligne affichée sans barre déroulante
        /// </summary>
        private const int maxRows = 9;

        /// <summary>
        /// Instance de controle
        /// </summary>
        Controller controller;

        /// <summary>
        /// Objet source gérant la liste des personnels
        /// </summary>
        BindingSource bdgPersonnel = new BindingSource();

        /// <summary>
        /// Objet source gérant la liste des services
        /// </summary>
        BindingSource bdgDepts = new BindingSource();

        /// <summary>
        /// Contructeur : Initialise les éléments de l'interface de gestion du personnel
        /// </summary>
        public FrmPersonnel(Controller controller)
        {
            this.controller = controller;
            InitializeComponent();
            BindDGVPersonnel();
            BindDGVDepts();
            BindActions();
            DrawDGVPersonnel();
            ResizeDGVPersonnel();
            TogglePersFields();
            ToggleButtons();
        }

        /// <summary>
        /// Recherche l'action demandée et prépare l'interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboAction.SelectedIndex == -1)
            {
                cboAction.Text = "Gérer le personnel";
            }
            else
            {
                if (CheckDGVIndex())
                {
                    ToggleSelection();
                    ToggleButtons();
                    // Ajouter
                    if (cboAction.SelectedIndex == 0)
                    {
                        TogglePersFields();
                        cboDept.Text = "Choissisez un service";
                    }
                    // Supprimer || Afficher
                    if (cboAction.SelectedIndex == 1 || cboAction.SelectedIndex == 3)
                    {
                        GetPersFields();
                    }
                    // Modifier
                    if (cboAction.SelectedIndex == 2)
                    {
                        TogglePersFields();
                        GetPersFields();
                    }
                }
                
            }
        }

        /// <summary>
        /// Verifie et valide l'action demandée 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValid_Click(object sender, EventArgs e)
        {
            // Récupérer personnel
            Personnel personnel = (Personnel)bdgPersonnel.List[bdgPersonnel.Position];
            // Afficher
            if (cboAction.SelectedIndex == 3)
            {
                controller.OpenFrmAbsence(this, personnel);
                ResetForm();
            }
            // Supprimer 
            else if (cboAction.SelectedIndex == 1)
            {
                if (ConfirmAction("Supprimer le personnel n° " + personnel.GetIdPersonnel + " : " + personnel.GetFirstName + " " + personnel.GetLastName + " ?", "Supprimer"))
                {
                    controller.DeleteAllAbsences(personnel.GetIdPersonnel);
                    controller.DeletePersonnel(personnel);
                    ResetForm();
                    bdgPersonnel.MoveFirst();
                }
            }
            // Verifier champs
            else if (CheckPersFields())
            {
                // Récupérer service
                Dept dept = (Dept)bdgDepts.List[bdgDepts.Position];
                // Modifier
                if (cboAction.SelectedIndex == 2)
                {
                    if (ConfirmAction("Modifier le personnel n° " + personnel.GetIdPersonnel + " : " + personnel.GetFirstName + " " + personnel.GetLastName + " ?", "Modifier"))
                    {
                        Personnel persUp = new Personnel(personnel.GetIdPersonnel, txtLastName.Text, txtFirstName.Text, txtPhone.Text, txtMail.Text, dept.GetIdDept, dept.GetName);
                        controller.UpdatePersonnel(persUp);
                        ResetForm();
                    }
                }
                // Ajouter
                if (cboAction.SelectedIndex == 0)
                {
                    Personnel persAdd = new Personnel(0, txtLastName.Text, txtFirstName.Text, txtPhone.Text, txtMail.Text, dept.GetIdDept, dept.GetName);
                    controller.AddPersonnel(persAdd);
                    ResetForm();
                    bdgPersonnel.MoveLast();
                }
            }            
        }

        /// <summary>
        /// Annule l'action et réinitialise l'interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        /// <summary>
        /// Initialise la grille de données des personnels
        /// </summary>
        private void BindDGVPersonnel()
        {
            List<Personnel> personnel = controller.GetPersonnel();
            bdgPersonnel.DataSource = personnel;
            dgvPersonnel.DataSource = bdgPersonnel;
        }

        /// <summary>
        /// Initialise la grille de données des services
        /// </summary>
        private void BindDGVDepts()
        {
            List<Dept> depts = controller.GetDepts();
            bdgDepts.DataSource = depts;
            cboDept.DataSource = bdgDepts;
            cboDept.Text = "";
        }

        /// <summary>
        /// Initialise les commandes possibles dans l'interface
        /// </summary>
        private void BindActions()
        {
            cboAction.Items.Clear();
            cboAction.Text = "Gérer les personnels";
            cboAction.Items.Add("Ajouter");
            cboAction.Items.Add("Supprimer");
            cboAction.Items.Add("Modifier");
            cboAction.Items.Add("Afficher les absences");
        }

        /// <summary>
        /// Redessine la grille de données des personnels
        /// </summary>
        private void DrawDGVPersonnel()
        {
            dgvPersonnel.Columns["GetIdPersonnel"].HeaderText = "ID";
            dgvPersonnel.Columns["GetLastName"].HeaderText = "Nom";
            dgvPersonnel.Columns["GetFirstName"].HeaderText = "Prénom";
            dgvPersonnel.Columns["GetPhone"].HeaderText = "Téléphone";
            dgvPersonnel.Columns["GetMail"].HeaderText = "Adresse Email";
            dgvPersonnel.Columns["GetIdDept"].Visible = false;
            dgvPersonnel.Columns["GetDept"].HeaderText = "Service";
            dgvPersonnel.Columns["GetDept"].DisplayIndex = 1;
            dgvPersonnel.Columns["GetDept"].Width = fieldWidthUnit * 5;
            dgvPersonnel.Columns["GetIdPersonnel"].Width = fieldWidthUnit * 2;
            dgvPersonnel.Columns["GetLastName"].Width = fieldWidthUnit * 5;
            dgvPersonnel.Columns["GetFirstName"].Width = fieldWidthUnit * 5;
            dgvPersonnel.Columns["GetPhone"].Width = fieldWidthUnit * 5;
            dgvPersonnel.Columns["GetMail"].Width = fieldWidthUnit * 7;
        }

        /// <summary>
        /// Defini la taille du champs Adresse Email selon le nombre de ligne dans la grille de données
        /// </summary>
        private void ResizeDGVPersonnel() {
            if (dgvPersonnel.RowCount > maxRows)
            {
                dgvPersonnel.Columns["GetMail"].Width = fieldWidthUnit *7;
            }
            else
            {
                dgvPersonnel.Columns["GetMail"].Width = fieldWidthUnit * 8;
            }
        }

        /// <summary>
        /// Réinitialise l'interface
        /// </summary>
        private void ResetForm()
        {
            ToggleSelection();
            ToggleButtons();
            BindDGVPersonnel();
            ResizeDGVPersonnel();
            cboAction.SelectedIndex = -1;
            txtLastName.Enabled = false;
            txtFirstName.Enabled = false;
            txtPhone.Enabled = false;
            txtMail.Enabled = false;
            cboDept.Enabled = false;
            txtLastName.Text = "";
            txtFirstName.Text = "";
            txtPhone.Text = "";
            txtMail.Text = "";
            cboDept.SelectedIndex = -1;
            cboDept.Text = "";
        }

        /// <summary>
        /// Active ou désactive les champs d'informations/ de saisie du personnel
        /// </summary>
        private void TogglePersFields()
        {
            txtLastName.Enabled = !txtLastName.Enabled;
            txtFirstName.Enabled = !txtFirstName.Enabled;
            txtPhone.Enabled = !txtPhone.Enabled;
            txtMail.Enabled = !txtMail.Enabled;
            cboDept.Enabled = !cboDept.Enabled;
        }

        /// <summary>
        /// Active ou désactive les boutons
        /// </summary>
        private void ToggleButtons()
        {
            btnCancel.Enabled = !btnCancel.Enabled;
            btnValid.Enabled = !btnValid.Enabled;
        }

        /// <summary>
        /// Active ou désactive les champs de sélection
        /// </summary>
        private void ToggleSelection()
        {
            dgvPersonnel.Enabled = !dgvPersonnel.Enabled;
            cboAction.Enabled = !cboAction.Enabled;
        }

        /// <summary>
        /// Récupère le personnel selectionné et affiche ses informations dans les champs
        /// </summary>
        private void GetPersFields()
        {
            Personnel pers = (Personnel)bdgPersonnel.List[bdgPersonnel.Position];
            txtLastName.Text = pers.GetLastName;
            txtFirstName.Text = pers.GetFirstName;
            txtPhone.Text = pers.GetPhone;
            txtMail.Text = pers.GetMail;
            cboDept.Text = pers.GetDept;
        }

        /// <summary>
        /// Verifie que tous les champs sont remplis et que le service choisi existe
        /// </summary>
        /// <returns>Vrai ou Faux</returns>
        private bool CheckPersFields()
        {
            if (txtLastName.Text.Equals("") || txtFirstName.Text.Equals("") || txtPhone.Text.Equals("") || txtMail.Text.Equals("") || cboDept.Text.Equals(""))
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Saisie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            string value = cboDept.Text;
            cboDept.Text = "";
            int index = cboDept.FindString(value);
            cboDept.Text = value;
            if (index < 0 || cboDept.SelectedIndex < 0)
            {
                MessageBox.Show("Choisissez un service existant.", "Service", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Réinitialise le texte de liste d'action et informe l'utlisateur si aucun élément de la grille de données n'est selectionné
        /// </summary>
        /// <returns>Vrai ou Faux</returns>
        private bool CheckDGVIndex()
        {
            if (dgvPersonnel.RowCount < 1)
            {
                MessageBox.Show("Aucun personnel n'est selectionné.", "Personnel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ToggleSelection();
                ToggleButtons();
                cboAction.SelectedIndex = -1;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Demande la confirmation de poursuivre l'action 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns>Vrai ou Faux</returns>
        private bool ConfirmAction(string message, string title)
        {
            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }
            return false;
        }     
    }
}
