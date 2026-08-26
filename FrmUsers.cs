using System;
using System.Drawing;
using System.Windows.Forms;
using NovaGestion.UI;
using NovaGestion.Data;

namespace NovaGestion
{
    public partial class FrmUsers : Form
    {
        private Panel pnlBody = null!;
        private RoundedButton btnTabUsers = null!;
        private RoundedButton btnTabPartenaires = null!;

        public FrmUsers()
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
            this.MinimizeBox = true;
            this.MinimumSize = new Size(800, 550);

            this.Controls.Add(BuildBody());
            this.Controls.Add(BuildFooter());
            this.Controls.Add(BuildHeader());

            ShowUsersTab();
        }

        // ============================================================
        // HEADER / FOOTER
        // ============================================================
        private Panel BuildHeader()
        {
            Panel pnl = new GradientPanel { Dock = DockStyle.Top, Height = 70, ColorStart = Theme.Maroon, ColorEnd = Theme.MaroonDark };

            Label lblBadge = new Label
            {
                Text = "N",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(120, 0, 0),
                Size = new Size(34, 34),
                Location = new Point(20, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // إضافة UseMnemonic = false لظهر حرف & بشكل صحيح
            Label lblTitle = new Label
            {
                Text = "Utilisateurs & Partenaires",
                UseMnemonic = false,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(64, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            pnl.Controls.Add(lblBadge);
            pnl.Controls.Add(lblTitle);
            return pnl;
        }

        private Panel BuildFooter()
        {
            Panel pnl = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = Color.White };
            Panel line = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Theme.BorderGray };

            RoundedButton btnClose = new RoundedButton
            {
                Text = "Fermer",
                Size = new Size(110, 38),
                Font = Theme.FontValue,
                BackColor = Theme.BtnLightBg,
                ForeColor = Theme.TextDark
            };
            btnClose.Click += (s, e) => this.Close();

            void Reposition() => btnClose.Location = new Point(pnl.Width - btnClose.Width - 20, 13);
            pnl.Resize += (s, e) => Reposition();
            pnl.HandleCreated += (s, e) => Reposition();

            pnl.Controls.Add(btnClose);
            pnl.Controls.Add(line);
            return pnl;
        }

        // ============================================================
        // Tabs (Utilisateurs / Partenaires)
        // ============================================================
        private Panel BuildBody()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, BackColor = Theme.PageBg, Padding = new Padding(20) };

            Panel pnlTabs = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Theme.PageBg };
            btnTabUsers = new RoundedButton { Text = "👤 Utilisateurs", Location = new Point(0, 0), Size = new Size(160, 38), Font = Theme.FontBold, BackColor = Theme.Maroon, ForeColor = Color.White };
            btnTabPartenaires = new RoundedButton { Text = "🤝 Partenaires", Location = new Point(170, 0), Size = new Size(160, 38), Font = Theme.FontValue, BackColor = Theme.BtnLightBg, ForeColor = Theme.TextDark };
            btnTabUsers.Click += (s, e) => ShowUsersTab();
            btnTabPartenaires.Click += (s, e) => ShowPartenairesTab();
            pnlTabs.Controls.AddRange(new Control[] { btnTabUsers, btnTabPartenaires });

            pnlBody = new Panel { Dock = DockStyle.Fill, BackColor = Theme.PageBg, Padding = new Padding(0, 15, 0, 0) };

            root.Controls.Add(pnlBody);
            root.Controls.Add(pnlTabs);
            return root;
        }

        private void ShowUsersTab()
        {
            btnTabUsers.BackColor = Theme.Maroon; btnTabUsers.ForeColor = Color.White; btnTabUsers.Font = Theme.FontBold;
            btnTabPartenaires.BackColor = Theme.BtnLightBg; btnTabPartenaires.ForeColor = Theme.TextDark; btnTabPartenaires.Font = Theme.FontValue;
            pnlBody.Controls.Clear();
            pnlBody.Controls.Add(BuildUsersView());
        }

        private void ShowPartenairesTab()
        {
            btnTabPartenaires.BackColor = Theme.Maroon; btnTabPartenaires.ForeColor = Color.White; btnTabPartenaires.Font = Theme.FontBold;
            btnTabUsers.BackColor = Theme.BtnLightBg; btnTabUsers.ForeColor = Theme.TextDark; btnTabUsers.Font = Theme.FontValue;
            pnlBody.Controls.Clear();
            pnlBody.Controls.Add(BuildPartenairesView());
        }

        // ============================================================
        // وحدة UTILISATEURS
        // ============================================================
        private Panel BuildUsersView()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, BackColor = Theme.PageBg };

            RoundedPanel card = new RoundedPanel { Dock = DockStyle.Top, Height = 140 };
            Label lblCardTitle = new Label { Text = "Ajouter un utilisateur", Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(16, 12), AutoSize = true };
            card.Controls.Add(lblCardTitle);

            Label l1 = MakeLabel("Nom complet", 20, 44); TextBox txtNom = MakeTextBox(20, 63, 160);
            Label l2 = MakeLabel("Login", 195, 44); TextBox txtLogin = MakeTextBox(195, 63, 120);
            Label l3 = MakeLabel("Mot de passe", 330, 44); TextBox txtPass = MakeTextBox(330, 63, 120); txtPass.UseSystemPasswordChar = true;
            Label l4 = MakeLabel("Rôle", 465, 44);
            ComboBox cmbRole = new ComboBox { Location = new Point(465, 63), Size = new Size(140, 28), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new object[] { "Administrateur", "Gestionnaire", "Consultation" });
            cmbRole.SelectedIndex = 0;
            Label l5 = MakeLabel("Service", 620, 44); TextBox txtService = MakeTextBox(620, 63, 140);

            RoundedButton btnSave = new RoundedButton
            {
                Text = "+ Enregistrer",
                Location = new Point(20, 98),
                Size = new Size(160, 32),
                Font = Theme.FontButton,
                BackColor = Theme.Maroon,
                ForeColor = Color.White
            };

            card.Controls.AddRange(new Control[] { l1, txtNom, l2, txtLogin, l3, txtPass, l4, cmbRole, l5, txtService, btnSave });

            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 15, BackColor = Theme.PageBg };

            DataGridView dgv = MakeGrid();
            dgv.Columns.Add("Nom", "Nom complet");
            dgv.Columns.Add("Login", "Login");
            dgv.Columns.Add("Role", "Rôle");
            dgv.Columns.Add("Service", "Service");
            foreach (var u in AppData.Users)
            {
                dgv.Rows.Add(u.Nom, u.Login, u.Role, u.Service);
            }

            Panel gridContainer = new Panel { Dock = DockStyle.Fill };
            gridContainer.Controls.Add(dgv);

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtLogin.Text))
                {
                    MessageBox.Show("الاسم والـ Login إجباريين.", "معلومات ناقصة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                AppData.Users.Add(new UserItem
                {
                    Nom = txtNom.Text.Trim(),
                    Login = txtLogin.Text.Trim(),
                    Role = cmbRole.SelectedItem?.ToString() ?? "",
                    Service = txtService.Text.Trim()
                });
                dgv.Rows.Add(txtNom.Text.Trim(), txtLogin.Text.Trim(), cmbRole.SelectedItem?.ToString(), txtService.Text.Trim());
                txtNom.Clear(); txtLogin.Clear(); txtPass.Clear(); txtService.Clear();
            };

            root.Controls.Add(gridContainer);
            root.Controls.Add(spacer);
            root.Controls.Add(card);
            return root;
        }

        // ============================================================
        // وحدة PARTENAIRES
        // ============================================================
        private Panel BuildPartenairesView()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, BackColor = Theme.PageBg };

            RoundedPanel card = new RoundedPanel { Dock = DockStyle.Top, Height = 200 };
            Label lblCardTitle = new Label { Text = "Ajouter un partenaire", Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(16, 12), AutoSize = true };
            card.Controls.Add(lblCardTitle);

            Label l1 = MakeLabel("Raison sociale", 20, 44); TextBox txtRaison = MakeTextBox(20, 63, 200);
            Label l2 = MakeLabel("Type", 235, 44);
            ComboBox cmbType = new ComboBox { Location = new Point(235, 63), Size = new Size(140, 28), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new object[] { "Fournisseur", "Client", "Partenaire" });
            cmbType.SelectedIndex = 0;
            Label l3 = MakeLabel("NIF", 390, 44); TextBox txtNif = MakeTextBox(390, 63, 110);
            Label l4 = MakeLabel("RC", 515, 44); TextBox txtRc = MakeTextBox(515, 63, 110);
            Label l5 = MakeLabel("NIS", 640, 44); TextBox txtNis = MakeTextBox(640, 63, 100);

            Label l6 = MakeLabel("Téléphone", 20, 99); TextBox txtTel = MakeTextBox(20, 118, 140);
            Label l7 = MakeLabel("Email", 175, 99); TextBox txtEmail = MakeTextBox(175, 118, 190);
            Label l8 = MakeLabel("Adresse", 380, 99); TextBox txtAdresse = MakeTextBox(380, 118, 360);

            card.Controls.AddRange(new Control[] { l1, txtRaison, l2, cmbType, l3, txtNif, l4, txtRc, l5, txtNis, l6, txtTel, l7, txtEmail, l8, txtAdresse });

            RoundedButton btnSave = new RoundedButton { Text = "+ Enregistrer", Location = new Point(20, 158), Size = new Size(160, 32), Font = Theme.FontButton, BackColor = Theme.Maroon, ForeColor = Color.White };
            card.Controls.Add(btnSave);

            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 15, BackColor = Theme.PageBg };

            DataGridView dgv = MakeGrid();
            dgv.Columns.Add("Raison", "Raison Sociale");
            dgv.Columns.Add("Type", "Type");
            dgv.Columns.Add("NIF", "NIF");
            dgv.Columns.Add("RC", "RC");
            dgv.Columns.Add("Tel", "Téléphone");
            foreach (var p in AppData.Partenaires)
            {
                dgv.Rows.Add(p.RaisonSociale, p.Type, p.NIF, p.RC, p.Telephone);
            }

            Panel gridContainer = new Panel { Dock = DockStyle.Fill };
            gridContainer.Controls.Add(dgv);

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtRaison.Text))
                {
                    MessageBox.Show("الـ Raison sociale إجبارية.", "معلومات ناقصة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                AppData.Partenaires.Add(new PartenaireItem
                {
                    RaisonSociale = txtRaison.Text.Trim(),
                    Type = cmbType.SelectedItem?.ToString() ?? "",
                    NIF = txtNif.Text.Trim(),
                    RC = txtRc.Text.Trim(),
                    NIS = txtNis.Text.Trim(),
                    Telephone = txtTel.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Adresse = txtAdresse.Text.Trim()
                });
                dgv.Rows.Add(txtRaison.Text.Trim(), cmbType.SelectedItem?.ToString(), txtNif.Text.Trim(), txtRc.Text.Trim(), txtTel.Text.Trim());
                txtRaison.Clear(); txtNif.Clear(); txtRc.Clear(); txtNis.Clear(); txtTel.Clear(); txtEmail.Clear(); txtAdresse.Clear();
            };

            root.Controls.Add(gridContainer);
            root.Controls.Add(spacer);
            root.Controls.Add(card);
            return root;
        }

        // ============================================================
        // Helpers
        // ============================================================
        private Label MakeLabel(string text, int x, int y)
        {
            return new Label { Text = text, UseMnemonic = false, Font = Theme.FontLabel, ForeColor = Theme.TextGray, Location = new Point(x, y), AutoSize = true, BackColor = Color.Transparent };
        }

        private TextBox MakeTextBox(int x, int y, int width)
        {
            return new TextBox { Location = new Point(x, y), Size = new Size(width, 28), Font = Theme.FontValue, BorderStyle = BorderStyle.FixedSingle };
        }

        private DataGridView MakeGrid()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 36 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = Theme.FontValue
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Theme.Maroon;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 38;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230);
            dgv.DefaultCellStyle.SelectionForeColor = Theme.Maroon;
            return dgv;
        }
    }
}