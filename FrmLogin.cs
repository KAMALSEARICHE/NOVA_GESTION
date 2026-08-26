using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NovaGestion.UI;

namespace NovaGestion
{
    public partial class FrmLogin : Form
    {
        private Panel pnlHeader = null!;
        private RoundedPanel pnlCard = null!;
        private Button btnTabUser = null!;
        private Button btnTabAdmin = null!;
        private ComboBox cmbIdentifiant = null!;
        private TextBox txtPassword = null!;
        private Button btnLogin = null!;

        private bool _isAdminMode;
        private bool _loginSucceeded;

        public FrmLogin()
        {
            InitializeComponent();
            BuildUI();
            AppAssets.ApplyIcon(this);
            this.FormClosed += (s, e) =>
            {
                if (!_loginSucceeded) Application.Exit();
            };
        }

        private void BuildUI()
        {
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;

            this.MinimumSize = new Size(420, 570);
            this.Size = new Size(420, 570);

            // ===== Header =====
            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 135, BackColor = Color.White };

            // 1. الشريط الفاصل باللون الذهبي الممتاز
            Panel flagBar = new Panel { Dock = DockStyle.Bottom, Height = 3, BackColor = Color.FromArgb(212, 175, 55) };

            PictureBox picLogo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            if (AppAssets.LogoFull != null)
            {
                picLogo.Image = AppAssets.LogoFull;
                picLogo.Size = new Size(350, 115);
            }
            else
            {
                picLogo.Size = new Size(200, 60);
                LogoBadge fallbackBadge = new LogoBadge { Size = new Size(56, 56), Letter = "N", BackColor = Color.White };
                Label fallbackText = new Label { Text = "NovaGestion", Font = new Font("Segoe UI", 20F, FontStyle.Bold), ForeColor = Theme.Maroon, AutoSize = true };
                pnlHeader.Controls.Add(fallbackBadge);
                pnlHeader.Controls.Add(fallbackText);
                fallbackBadge.Location = new Point(150, 45);
                fallbackText.Location = new Point(216, 63);
            }

            pnlHeader.Controls.Add(flagBar);
            pnlHeader.Controls.Add(picLogo);

            void RepositionHeader()
            {
                picLogo.Location = new Point((pnlHeader.Width - picLogo.Width) / 2, 10);
            }
            pnlHeader.Resize += (s, e) => RepositionHeader();
            RepositionHeader();

            // ===== Card =====
            pnlCard = new RoundedPanel
            {
                Size = new Size(340, 290),
                BackColor = Color.White,
                BorderColor = Color.FromArgb(225, 225, 230)
            };

            // 2. أزرار التبويب بحواف دائرية أنيقة
            btnTabUser = new Button
            {
                Text = "👤  Utilisateur",
                Location = new Point(20, 20),
                Size = new Size(145, 38),
                FlatStyle = FlatStyle.Flat,
                Font = Theme.FontBold,
                BackColor = Color.FromArgb(245, 245, 247),
                ForeColor = Theme.TextDark,
                Cursor = Cursors.Hand
            };
            btnTabUser.FlatAppearance.BorderSize = 0;
            ApplyRoundedRegion(btnTabUser, 10);

            btnTabAdmin = new Button
            {
                Text = "🛡  Administrateur",
                Location = new Point(175, 20),
                Size = new Size(145, 38),
                FlatStyle = FlatStyle.Flat,
                Font = Theme.FontValue,
                BackColor = Color.White,
                ForeColor = Theme.TextGray,
                Cursor = Cursors.Hand
            };
            btnTabAdmin.FlatAppearance.BorderSize = 0;
            ApplyRoundedRegion(btnTabAdmin, 10);

            btnTabUser.Click += (s, e) => SwitchTab(false);
            btnTabAdmin.Click += (s, e) => SwitchTab(true);

            Label lblIdentifiant = new Label
            {
                Text = "Identifiant",
                Font = Theme.FontLabel,
                ForeColor = Theme.TextGray,
                Location = new Point(20, 75),
                AutoSize = true
            };
            cmbIdentifiant = new ComboBox
            {
                Location = new Point(20, 97),
                Size = new Size(300, 30),
                Font = Theme.FontValue,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };

            Label lblPassword = new Label
            {
                Text = "Mot de passe",
                Font = Theme.FontLabel,
                ForeColor = Theme.TextGray,
                Location = new Point(20, 142),
                AutoSize = true
            };
            txtPassword = new TextBox
            {
                Location = new Point(20, 164),
                Size = new Size(300, 30),
                Font = Theme.FontValue,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e);
            };

            // 3. زر تسجيل الدخول بلون كحلي فاخر وحواف دائرية
            btnLogin = new Button
            {
                Text = "Se connecter",
                Location = new Point(20, 218),
                Size = new Size(300, 44),
                FlatStyle = FlatStyle.Flat,
                Font = Theme.FontButton,
                BackColor = Color.FromArgb(20, 35, 60), // لون كحلي فاخر متناسق مع الشعار
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(128, 20, 40); // لون عنابي عند تمرير الماوس
            ApplyRoundedRegion(btnLogin, 10);
            btnLogin.Click += BtnLogin_Click;

            pnlCard.Controls.AddRange(new Control[]
            {
                btnTabUser, btnTabAdmin, lblIdentifiant, cmbIdentifiant, lblPassword, txtPassword, btnLogin
            });

            Label lblFooter = new Label
            {
                Text = "© 2026 NovaGestion",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Theme.TextMuted,
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.Add(pnlCard);
            this.Controls.Add(lblFooter);
            this.Controls.Add(pnlHeader);

            this.Resize += (s, e) => CenterCard();
            SwitchTab(false);
            CenterCard();
        }

        // دالة قص الحواف بشكل دائري سلس وبدون تشوه
        private void ApplyRoundedRegion(Control control, int radius)
        {
            control.SizeChanged += (s, e) =>
            {
                using GraphicsPath path = new GraphicsPath();
                int d = radius * 2;
                path.AddArc(0, 0, d, d, 180, 90);
                path.AddArc(control.Width - d, 0, d, d, 270, 90);
                path.AddArc(control.Width - d, control.Height - d, d, d, 0, 90);
                path.AddArc(0, control.Height - d, d, d, 90, 90);
                path.CloseFigure();
                control.Region = new Region(path);
            };
        }

        private void CenterCard()
        {
            if (pnlCard == null || pnlHeader == null) return;
            int yLocation = pnlHeader.Bottom + 15;
            pnlCard.Location = new Point((this.ClientSize.Width - pnlCard.Width) / 2, yLocation);
        }

        private void SwitchTab(bool adminMode)
        {
            _isAdminMode = adminMode;

            btnTabUser.Font = adminMode ? Theme.FontValue : Theme.FontBold;
            btnTabUser.ForeColor = adminMode ? Theme.TextGray : Theme.TextDark;
            btnTabUser.BackColor = adminMode ? Color.White : Color.FromArgb(245, 245, 247);

            btnTabAdmin.Font = adminMode ? Theme.FontBold : Theme.FontValue;
            btnTabAdmin.ForeColor = adminMode ? Theme.TextDark : Theme.TextGray;
            btnTabAdmin.BackColor = adminMode ? Color.FromArgb(245, 245, 247) : Color.White;

            cmbIdentifiant.Items.Clear();
            if (adminMode)
            {
                cmbIdentifiant.Items.Add("ADMIN");
            }
            else
            {
                cmbIdentifiant.Items.AddRange(new object[] { "A001", "A002", "A003" });
            }
            cmbIdentifiant.SelectedIndex = 0;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string identifiant = cmbIdentifiant.SelectedItem?.ToString() ?? "";
            string password = txtPassword.Text ?? "";

            bool ok = _isAdminMode
                ? (identifiant == "ADMIN" && password == "1234")
                : (!string.IsNullOrWhiteSpace(identifiant) && password == "1234");

            if (ok)
            {
                _loginSucceeded = true;
                string displayName = _isAdminMode ? "Administrateur" : "Utilisateur";
                this.Hide();

                FrmMain mainForm = new FrmMain(displayName, identifiant);
                mainForm.FormClosed += (s2, e2) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur de connexion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}