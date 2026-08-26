using System;
using System.Drawing;
using System.Windows.Forms;
using NovaGestion.UI;
using NovaGestion.Data;

namespace NovaGestion
{
    public partial class FrmContracts : Form
    {
        // ===== حقول القسم 1: Références =====
        private TextBox txtAutorisation = null!;
        private DateTimePicker dtpDateContrat = null!;
        private TextBox txtConsultation = null!;
        private NumericUpDown numDureeJours = null!;
        private TextBox txtNumContrat = null!;
        private ComboBox cmbStatut = null!;

        // ===== حقول القسم 2: Type & Partenaire =====
        private ComboBox cmbTypeContrat = null!;
        private TextBox txtRC = null!;
        private TextBox txtRaisonSociale = null!;
        private TextBox txtArticle = null!;
        private TextBox txtNIF = null!;
        private TextBox txtMobile = null!;
        private TextBox txtNIS = null!;
        private TextBox txtEmail = null!;
        private TextBox txtAdresse = null!;
        private TextBox txtRibCcp = null!;

        // ===== حقول القسم 3: Objet & Montant =====
        private TextBox txtDomaine = null!;
        private TextBox txtMontantTTC = null!;
        private TextBox txtObservations = null!;

        // ===== القسم 4: Retenue de Garantie =====
        private CheckBox chkRetenueGarantie = null!;
        private NumericUpDown numPourcentageRetenue = null!;
        private Label lblMontantRetenueCalculated = null!;

        // ===== Location - Équipements / Matériels =====
        private DataGridView dgvMateriels = null!;
        private DataGridViewComboBoxColumn colTypeMateriel = null!;

        private Panel pnlScroll = null!;
        private const int CardWidth = 700;
        private const int LeftX = 20;
        private const int RightX = 360;
        private const int FieldW = 300;
        private const int FullW = 640;

        public FrmContracts()
        {
            InitializeComponent();
            AppAssets.ApplyIcon(this);
            BuildUI();
        }

        private void BuildUI()
        {
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(650, 500);

            this.Controls.Add(BuildScrollArea());
            this.Controls.Add(BuildFooter());
            this.Controls.Add(BuildHeader());
        }

        // ============================================================
        // HEADER عنابي علوي
        // ============================================================
        private Panel BuildHeader()
        {
            Panel pnl = new GradientPanel { Dock = DockStyle.Top, Height = 70, ColorStart = Theme.Maroon, ColorEnd = Theme.MaroonDark };

            LogoBadge logo = new LogoBadge { Size = new Size(34, 34), Letter = "N", Location = new Point(20, 18), BackColor = Color.Transparent };
            Label lblTitle = new Label
            {
                Text = "Nouveau Contrat",
                UseMnemonic = false,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(64, 20),
                AutoSize = true
            };
            pnl.Controls.Add(logo);
            pnl.Controls.Add(lblTitle);
            return pnl;
        }

        // ============================================================
        // FOOTER (Annuler / Créer le contrat)
        // ============================================================
        private Panel BuildFooter()
        {
            Panel pnl = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            Panel topLine = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.BorderGray };

            RoundedButton btnCreate = new RoundedButton
            {
                Text = "💾  Créer le contrat",
                Size = new Size(180, 42),
                Font = Theme.FontButton,
                BackColor = Theme.Maroon,
                ForeColor = Color.White
            };
            RoundedButton btnCancel = new RoundedButton
            {
                Text = "Annuler",
                Size = new Size(110, 42),
                Font = Theme.FontValue,
                BackColor = Theme.BtnLightBg,
                ForeColor = Theme.TextDark
            };

            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            void Reposition()
            {
                btnCreate.Location = new Point(pnl.Width - btnCreate.Width - 20, 14);
                btnCancel.Location = new Point(btnCreate.Left - btnCancel.Width - 10, 14);
            }
            pnl.Resize += (s, e) => Reposition();

            pnl.Controls.Add(btnCancel);
            pnl.Controls.Add(btnCreate);
            pnl.Controls.Add(topLine);
            pnl.HandleCreated += (s, e) => Reposition();
            return pnl;
        }

        // ============================================================
        // منطقة التمرير
        // ============================================================
        private System.Collections.Generic.List<RoundedPanel> _cards = new();

        private Panel BuildScrollArea()
        {
            pnlScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Theme.PageBg };

            int y = 15;
            RoundedPanel card1 = BuildCard1References(ref y);
            RoundedPanel card2 = BuildCard2Partenaire(ref y);
            RoundedPanel card3 = BuildCard3Objet(ref y);
            RoundedPanel card4 = BuildCard4Retenue(ref y);
            RoundedPanel card5 = BuildCard5Location(ref y);

            _cards.AddRange(new[] { card1, card2, card3, card4, card5 });

            pnlScroll.Controls.Add(card1);
            pnlScroll.Controls.Add(card2);
            pnlScroll.Controls.Add(card3);
            pnlScroll.Controls.Add(card4);
            pnlScroll.Controls.Add(card5);

            pnlScroll.Resize += (s, e) => RecenterCards();
            RecenterCards();

            return pnlScroll;
        }

        private void RecenterCards()
        {
            int x = Math.Max(20, (pnlScroll.ClientSize.Width - CardWidth) / 2);
            foreach (var card in _cards)
            {
                card.Left = x;
            }
        }

        private RoundedPanel NewCard(int y, int height)
        {
            return new RoundedPanel { Location = new Point(20, y), Size = new Size(CardWidth, height) };
        }

        private void AddSectionTitle(RoundedPanel card, string number, string title)
        {
            NumberBadge badge = new NumberBadge { Number = number, Location = new Point(16, 14) };
            Label lbl = new Label
            {
                Text = title,
                UseMnemonic = false,
                Font = Theme.FontSectionTitle,
                ForeColor = Theme.TextDark,
                Location = new Point(48, 15),
                AutoSize = true
            };
            card.Controls.Add(badge);
            card.Controls.Add(lbl);
        }

        private Label AddFieldLabel(RoundedPanel card, string text, int x, int y)
        {
            Label lbl = new Label
            {
                Text = text,
                UseMnemonic = false,
                Font = Theme.FontLabel,
                ForeColor = Theme.TextGray,
                Location = new Point(x, y),
                AutoSize = true
            };
            card.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(RoundedPanel card, int x, int y, int width, bool multiline = false, int height = 28)
        {
            TextBox txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, multiline ? height : 28),
                Font = Theme.FontValue,
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = multiline
            };
            card.Controls.Add(txt);
            return txt;
        }

        // ============================================================
        // CARD 1: Références
        // ============================================================
        private RoundedPanel BuildCard1References(ref int y)
        {
            RoundedPanel card = NewCard(y, 265);
            AddSectionTitle(card, "1", "Références");

            AddFieldLabel(card, "N° Autorisation *", LeftX, 55);
            txtAutorisation = AddTextBox(card, LeftX, 74, FieldW);

            AddFieldLabel(card, "Date du Contrat", RightX, 55);
            dtpDateContrat = new DateTimePicker
            {
                Location = new Point(RightX, 74),
                Size = new Size(FieldW, 28),
                Font = Theme.FontValue,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            card.Controls.Add(dtpDateContrat);

            AddFieldLabel(card, "N° Consultation", LeftX, 120);
            txtConsultation = AddTextBox(card, LeftX, 139, FieldW);

            AddFieldLabel(card, "Durée (en jours) *", RightX, 120);
            numDureeJours = new NumericUpDown
            {
                Location = new Point(RightX, 139),
                Size = new Size(120, 28),
                Font = Theme.FontValue,
                Maximum = 3650,
                Value = 365
            };
            Label lblJours = new Label { Text = "jours", Location = new Point(RightX + 128, 145), AutoSize = true, ForeColor = Theme.TextGray, Font = Theme.FontLabel, UseMnemonic = false };
            card.Controls.AddRange(new Control[] { numDureeJours, lblJours });

            AddFieldLabel(card, "N° Contrat", LeftX, 185);
            txtNumContrat = AddTextBox(card, LeftX, 204, FieldW);

            AddFieldLabel(card, "Statut", RightX, 185);
            cmbStatut = new ComboBox
            {
                Location = new Point(RightX, 204),
                Size = new Size(FieldW, 28),
                Font = Theme.FontValue,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatut.Items.AddRange(new object[] { "ACTIF", "INACTIF", "RÉSILIÉ", "SUSPENDU" });
            cmbStatut.SelectedIndex = 0;
            card.Controls.Add(cmbStatut);

            y += card.Height + 15;
            return card;
        }

        // ============================================================
        // CARD 2: Type & Partenaire
        // ============================================================
        private RoundedPanel BuildCard2Partenaire(ref int y)
        {
            RoundedPanel card = NewCard(y, 435);
            AddSectionTitle(card, "2", "Type & Partenaire");

            AddFieldLabel(card, "Type de contrat *", LeftX, 55);

            cmbTypeContrat = new ComboBox
            {
                Location = new Point(LeftX, 74),
                Size = new Size(150, 28),
                Font = Theme.FontValue,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTypeContrat.Items.AddRange(new object[] { "STL", "PRESTATION", "LOCATION" });
            cmbTypeContrat.SelectedIndex = 0;
            cmbTypeContrat.SelectedIndexChanged += (s, e) => UpdateNumContratPattern();

            Button btnAddType = new Button
            {
                Text = "+",
                Location = new Point(LeftX + 155, 73),
                Size = new Size(30, 29),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Theme.BtnLightBg,
                FlatStyle = FlatStyle.Flat
            };
            btnAddType.Click += BtnAddType_Click;

            Button btnDelType = new Button
            {
                Text = "-",
                Location = new Point(LeftX + 190, 73),
                Size = new Size(30, 29),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Theme.BtnLightBg,
                FlatStyle = FlatStyle.Flat
            };
            btnDelType.Click += (s, e) =>
            {
                if (cmbTypeContrat.SelectedItem != null && cmbTypeContrat.Items.Count > 1)
                {
                    cmbTypeContrat.Items.Remove(cmbTypeContrat.SelectedItem);
                    cmbTypeContrat.SelectedIndex = 0;
                }
            };

            card.Controls.AddRange(new Control[] { cmbTypeContrat, btnAddType, btnDelType });

            AddFieldLabel(card, "RC", RightX, 55);
            txtRC = AddTextBox(card, RightX, 74, FieldW);

            AddFieldLabel(card, "Raison sociale *", LeftX, 120);
            txtRaisonSociale = AddTextBox(card, LeftX, 139, FieldW);

            AddFieldLabel(card, "Article", RightX, 120);
            txtArticle = AddTextBox(card, RightX, 139, FieldW);

            AddFieldLabel(card, "NIF", LeftX, 185);
            txtNIF = AddTextBox(card, LeftX, 204, FieldW);

            AddFieldLabel(card, "Mobile", RightX, 185);
            txtMobile = AddTextBox(card, RightX, 204, FieldW);

            AddFieldLabel(card, "NIS", LeftX, 250);
            txtNIS = AddTextBox(card, LeftX, 269, FieldW);

            AddFieldLabel(card, "Email", RightX, 250);
            txtEmail = AddTextBox(card, RightX, 269, FieldW);

            AddFieldLabel(card, "Adresse", LeftX, 315);
            txtAdresse = AddTextBox(card, LeftX, 334, FullW);

            AddFieldLabel(card, "RIB/CCP", LeftX, 375);
            txtRibCcp = AddTextBox(card, LeftX, 394, FullW);

            y += card.Height + 15;
            return card;
        }

        private void UpdateNumContratPattern()
        {
            string prefix = cmbTypeContrat.SelectedItem?.ToString() ?? "CT";
            string currentYear = DateTime.Now.ToString("yy");
            int count = AppData.Contrats.Count + 1;
            txtNumContrat.Text = $"{prefix}/{count:00}/{currentYear}/R/A70 DU";
        }

        private void BtnAddType_Click(object? sender, EventArgs e)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 320;
                prompt.Height = 160;
                prompt.Text = "Nouveau Type";
                prompt.StartPosition = FormStartPosition.CenterParent;

                Label lblType = new Label { Left = 20, Top = 15, Text = "Code/Nom du type:", AutoSize = true };
                TextBox txtType = new TextBox { Left = 20, Top = 35, Width = 260 };
                Button btnOk = new Button { Text = "Ajouter", Left = 190, Top = 75, Width = 90, DialogResult = DialogResult.OK };

                prompt.Controls.AddRange(new Control[] { lblType, txtType, btnOk });
                if (prompt.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtType.Text))
                {
                    string val = txtType.Text.Trim().ToUpper();
                    if (!cmbTypeContrat.Items.Contains(val))
                    {
                        cmbTypeContrat.Items.Add(val);
                        cmbTypeContrat.SelectedItem = val;
                    }
                }
            }
        }

        // ============================================================
        // CARD 3: Objet & Montant
        // ============================================================
        private RoundedPanel BuildCard3Objet(ref int y)
        {
            RoundedPanel card = NewCard(y, 205);
            AddSectionTitle(card, "3", "Objet & Montant");

            AddFieldLabel(card, "Domaine d'activité", LeftX, 55);
            txtDomaine = AddTextBox(card, LeftX, 74, FieldW);

            AddFieldLabel(card, "Montant TTC (DA) *", RightX, 55);
            txtMontantTTC = AddTextBox(card, RightX, 74, FieldW);
            txtMontantTTC.TextChanged += (s, e) => CalculateRetenueAmount();

            AddFieldLabel(card, "Observations", LeftX, 120);
            txtObservations = AddTextBox(card, LeftX, 139, FullW, multiline: true, height: 50);

            y += card.Height + 15;
            return card;
        }

        // ============================================================
        // CARD 4: Retenue de Garantie
        // ============================================================
        private RoundedPanel BuildCard4Retenue(ref int y)
        {
            RoundedPanel card = NewCard(y, 90);
            AddSectionTitle(card, "4", "Retenue de Garantie");

            chkRetenueGarantie = new CheckBox
            {
                Text = "Ce contrat a une retenue de garantie",
                Location = new Point(LeftX, 55),
                AutoSize = true,
                Font = Theme.FontValue,
                ForeColor = Theme.TextDark
            };

            Label lblTaux = new Label
            {
                Text = "Taux (%) :",
                Location = new Point(RightX, 56),
                AutoSize = true,
                Font = Theme.FontLabel,
                ForeColor = Theme.TextGray,
                UseMnemonic = false
            };

            numPourcentageRetenue = new NumericUpDown
            {
                Location = new Point(RightX + 65, 53),
                Size = new Size(60, 28),
                Font = Theme.FontValue,
                DecimalPlaces = 1,
                Maximum = 100,
                Value = 5.0m,
                Enabled = false
            };

            lblMontantRetenueCalculated = new Label
            {
                Text = "= 0.00 DA",
                Location = new Point(RightX + 135, 56),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Theme.Maroon
            };

            numPourcentageRetenue.ValueChanged += (s, e) => CalculateRetenueAmount();

            chkRetenueGarantie.CheckedChanged += (s, e) =>
            {
                numPourcentageRetenue.Enabled = chkRetenueGarantie.Checked;
                CalculateRetenueAmount();
            };

            card.Controls.AddRange(new Control[] { chkRetenueGarantie, lblTaux, numPourcentageRetenue, lblMontantRetenueCalculated });

            y += card.Height + 15;
            return card;
        }

        private void CalculateRetenueAmount()
        {
            if (chkRetenueGarantie.Checked && decimal.TryParse(txtMontantTTC.Text, out decimal montant))
            {
                decimal retenueVal = montant * (numPourcentageRetenue.Value / 100m);
                lblMontantRetenueCalculated.Text = $"= {retenueVal:N2} DA";
            }
            else
            {
                lblMontantRetenueCalculated.Text = "= 0.00 DA";
            }
        }

        // ============================================================
        // CARD 5: Location - Équipements / Matériels
        // ============================================================
        private RoundedPanel BuildCard5Location(ref int y)
        {
            RoundedPanel card = NewCard(y, 230);

            Label lblIcon = new Label { Text = "🚛", Font = new Font("Segoe UI", 12F), Location = new Point(16, 14), AutoSize = true };
            Label lblTitle = new Label { Text = "Location - Équipements / Matériels", UseMnemonic = false, Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(48, 15), AutoSize = true };

            RoundedButton btnAjouterMat = new RoundedButton
            {
                Text = "+ Matériel",
                Size = new Size(100, 32),
                Font = Theme.FontValue,
                BackColor = Theme.BtnLightBg,
                ForeColor = Theme.TextDark,
                Location = new Point(CardWidth - 116, 12)
            };
            btnAjouterMat.Click += (s, e) =>
            {
                dgvMateriels.Rows.Add("Camion", "30", "");
            };

            dgvMateriels = new DataGridView
            {
                Location = new Point(16, 55),
                Size = new Size(CardWidth - 32, 160),
                AllowUserToAddRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                Font = Theme.FontValue
            };
            dgvMateriels.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvMateriels.ColumnHeadersDefaultCellStyle.ForeColor = Theme.TextGray;
            dgvMateriels.ColumnHeadersDefaultCellStyle.Font = Theme.FontLabel;
            dgvMateriels.ColumnHeadersHeight = 30;

            colTypeMateriel = new DataGridViewComboBoxColumn
            {
                Name = "TypeMateriel",
                HeaderText = "Type de matériel (Camion, Grue...)",
                FlatStyle = FlatStyle.Flat
            };
            colTypeMateriel.Items.AddRange("Camion", "Grue", "Pelle mécanique", "Compacteur");

            dgvMateriels.Columns.Add(colTypeMateriel);
            dgvMateriels.Columns.Add("NbJours", "Nombre de jours");
            dgvMateriels.Columns.Add("Description", "Description / Immatriculation");

            card.Controls.AddRange(new Control[] { dgvMateriels, btnAjouterMat, lblTitle, lblIcon });

            y += card.Height + 15;
            return card;
        }

        // ============================================================
        // BtnCreate_Click
        // ============================================================
        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAutorisation.Text))
            {
                MessageBox.Show("Le champ « N° Autorisation » est obligatoire.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAutorisation.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRaisonSociale.Text))
            {
                MessageBox.Show("Le champ « Raison sociale » est obligatoire.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRaisonSociale.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMontantTTC.Text))
            {
                MessageBox.Show("Le champ « Montant TTC (DA) » est obligatoire.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMontantTTC.Focus();
                return;
            }

            decimal.TryParse(txtMontantTTC.Text, out decimal montant);
            string numContrat = string.IsNullOrWhiteSpace(txtNumContrat.Text)
                ? $"CT-{(AppData.Contrats.Count + 1):0000}"
                : txtNumContrat.Text.Trim();

            AppData.Contrats.Add(new ContratItem
            {
                NumContrat = numContrat,
                RaisonSociale = txtRaisonSociale.Text.Trim(),
                Type = cmbTypeContrat.SelectedItem?.ToString() ?? "",
                Statut = cmbStatut.SelectedItem?.ToString() ?? "",
                MontantTTC = montant,
                FactureDA = 0,
                DateContrat = dtpDateContrat.Value
            });

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}