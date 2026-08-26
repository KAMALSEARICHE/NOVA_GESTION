using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NovaGestion.UI;
using NovaGestion.Data;

namespace NovaGestion
{
    public partial class FrmAvenant : Form
    {
        private const int CardWidth = 560;
        private const int LeftX = 20;
        private const int RightX = 300;
        private const int FieldW = 240;
        private const int FullW = 520;

        private ComboBox cmbContrat = null!;
        private TextBox txtNumAvenant = null!;
        private DateTimePicker dtpDate = null!;
        private ComboBox cmbTypeModif = null!;
        private TextBox txtAncienMontant = null!;
        private TextBox txtNouveauMontant = null!;
        private NumericUpDown numAncienneDuree = null!;
        private NumericUpDown numNouvelleDuree = null!;
        private TextBox txtObjet = null!;
        private ComboBox cmbStatut = null!;
        private TextBox txtObservations = null!;

        public FrmAvenant()
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
            this.MinimumSize = new Size(550, 500);

            this.Controls.Add(BuildScrollArea());
            this.Controls.Add(BuildFooter());
            this.Controls.Add(BuildHeader());
        }

        private Panel BuildHeader()
        {
            Panel pnl = new GradientPanel { Dock = DockStyle.Top, Height = 70, ColorStart = Theme.Maroon, ColorEnd = Theme.MaroonDark };
            LogoBadge logo = new LogoBadge { Size = new Size(34, 34), Letter = "N", Location = new Point(20, 18) };
            Label lblTitle = new Label { Text = "Nouvel Avenant", Font = new Font("Segoe UI", 15F, FontStyle.Bold), ForeColor = Color.White, Location = new Point(64, 20), AutoSize = true };
            pnl.Controls.Add(logo);
            pnl.Controls.Add(lblTitle);
            return pnl;
        }

        private Panel BuildFooter()
        {
            Panel pnl = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            Panel topLine = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.BorderGray };

            RoundedButton btnCreate = new RoundedButton
            {
                Text = "💾  Créer l'avenant",
                Size = new Size(170, 42),
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
            pnl.HandleCreated += (s, e) => Reposition();

            pnl.Controls.Add(btnCancel);
            pnl.Controls.Add(btnCreate);
            pnl.Controls.Add(topLine);
            return pnl;
        }

        private Panel BuildScrollArea()
        {
            Panel pnlScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Theme.PageBg };

            RoundedPanel card = new RoundedPanel { Location = new Point(20, 15), Size = new Size(CardWidth, 470) };
            Label lblIcon = new Label { Text = "📑", Font = new Font("Segoe UI", 14F), Location = new Point(16, 12), AutoSize = true };
            Label lblSection = new Label { Text = "Informations de l'avenant", Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(48, 15), AutoSize = true };
            card.Controls.Add(lblIcon);
            card.Controls.Add(lblSection);

            AddLabel(card, "Contrat concerné *", LeftX, 55);
            cmbContrat = new ComboBox { Location = new Point(LeftX, 74), Size = new Size(FullW, 28), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (var c in AppData.Contrats)
            {
                cmbContrat.Items.Add($"{c.NumContrat} — {c.RaisonSociale}");
            }
            if (cmbContrat.Items.Count > 0) cmbContrat.SelectedIndex = 0;
            card.Controls.Add(cmbContrat);

            AddLabel(card, "N° Avenant *", LeftX, 120);
            txtNumAvenant = AddTextBox(card, LeftX, 139, FieldW);
            txtNumAvenant.Text = $"AV-{(AppData.Avenants.Count + 1):0000}";

            AddLabel(card, "Date de l'avenant", RightX, 120);
            dtpDate = new DateTimePicker { Location = new Point(RightX, 139), Size = new Size(FieldW, 28), Font = Theme.FontValue, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            card.Controls.Add(dtpDate);

            AddLabel(card, "Type de modification *", LeftX, 185);
            cmbTypeModif = new ComboBox { Location = new Point(LeftX, 204), Size = new Size(FieldW, 28), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTypeModif.Items.AddRange(new object[] { "Montant", "Durée", "Objet", "Partenaire", "Autre" });
            cmbTypeModif.SelectedIndex = 0;
            card.Controls.Add(cmbTypeModif);

            AddLabel(card, "Statut", RightX, 185);
            cmbStatut = new ComboBox { Location = new Point(RightX, 204), Size = new Size(FieldW, 28), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatut.Items.AddRange(new object[] { "En attente", "Validé", "Signé", "Rejeté" });
            cmbStatut.SelectedIndex = 0;
            card.Controls.Add(cmbStatut);

            AddLabel(card, "Ancien Montant (DA)", LeftX, 250);
            txtAncienMontant = AddTextBox(card, LeftX, 269, FieldW);

            AddLabel(card, "Nouveau Montant (DA)", RightX, 250);
            txtNouveauMontant = AddTextBox(card, RightX, 269, FieldW);

            AddLabel(card, "Ancienne Durée (mois)", LeftX, 315);
            numAncienneDuree = new NumericUpDown { Location = new Point(LeftX, 334), Size = new Size(FieldW, 28), Font = Theme.FontValue, Maximum = 120 };
            card.Controls.Add(numAncienneDuree);

            AddLabel(card, "Nouvelle Durée (mois)", RightX, 315);
            numNouvelleDuree = new NumericUpDown { Location = new Point(RightX, 334), Size = new Size(FieldW, 28), Font = Theme.FontValue, Maximum = 120 };
            card.Controls.Add(numNouvelleDuree);

            AddLabel(card, "Objet de la modification", LeftX, 380);
            txtObjet = AddTextBox(card, LeftX, 399, FullW, multiline: true, height: 45);

            AddLabel(card, "Observations", LeftX, 450);
            txtObservations = AddTextBox(card, LeftX, 469, FullW, multiline: true, height: 45);

            card.Size = new Size(CardWidth, 530);

            pnlScroll.Controls.Add(card);

            // نعاود نموقع البطاقة فـ وسط النافذة كل مرة يتبدل حجمها
            void Recenter() => card.Left = Math.Max(20, (pnlScroll.ClientSize.Width - CardWidth) / 2);
            pnlScroll.Resize += (s, e) => Recenter();
            Recenter();

            return pnlScroll;
        }

        private Label AddLabel(RoundedPanel card, string text, int x, int y)
        {
            Label lbl = new Label { Text = text, Font = Theme.FontLabel, ForeColor = Theme.TextGray, Location = new Point(x, y), AutoSize = true };
            card.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(RoundedPanel card, int x, int y, int width, bool multiline = false, int height = 28)
        {
            TextBox txt = new TextBox { Location = new Point(x, y), Size = new Size(width, multiline ? height : 28), Font = Theme.FontValue, BorderStyle = BorderStyle.FixedSingle, Multiline = multiline };
            card.Controls.Add(txt);
            return txt;
        }

        private void BtnCreate_Click(object? sender, EventArgs e)
        {
            if (cmbContrat.SelectedIndex < 0)
            {
                MessageBox.Show("Veuillez sélectionner le contrat concerné.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNumAvenant.Text))
            {
                MessageBox.Show("Le champ « N° Avenant » est obligatoire.", "Champ manquant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumAvenant.Focus();
                return;
            }

            ContratItem contrat = AppData.Contrats[cmbContrat.SelectedIndex];

            AppData.Avenants.Add(new AvenantItem
            {
                NumAvenant = txtNumAvenant.Text.Trim(),
                NumContratLie = contrat.NumContrat,
                RaisonSociale = contrat.RaisonSociale,
                TypeModification = cmbTypeModif.SelectedItem?.ToString() ?? "",
                DateAvenant = dtpDate.Value,
                AncienMontant = txtAncienMontant.Text.Trim(),
                NouveauMontant = txtNouveauMontant.Text.Trim(),
                AncienneDureeMois = (int)numAncienneDuree.Value,
                NouvelleDureeMois = (int)numNouvelleDuree.Value,
                Objet = txtObjet.Text.Trim(),
                Statut = cmbStatut.SelectedItem?.ToString() ?? "",
                Observations = txtObservations.Text.Trim()
            });

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
