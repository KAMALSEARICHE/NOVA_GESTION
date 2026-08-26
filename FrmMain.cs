using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NovaGestion.UI;
using NovaGestion.Data;

namespace NovaGestion
{
    /// <summary>
    /// الشاشة الرئيسية (Shell) لتطبيق NovaGestion — تحوي شريط جانبي (Sidebar) للتنقل بين
    /// كل وحدات النظام حسب خطة العمل: Dashboard, Contrats, Avenants, Courriers,
    /// Documents, Utilisateurs, Notifications, Rapports, Paramètres.
    /// </summary>
    public partial class FrmMain : Form
    {
        private readonly string _userDisplay;
        private readonly string _userCode;

        // --- هيكل الواجهة العام ---
        private Panel pnlSidebar = null!;
        private FlowLayoutPanel pnlNav = null!;
        private Panel pnlTopBar = null!;
        private Panel pnlContent = null!;
        private Label lblPageIcon = null!;
        private Label lblPageTitle = null!;
        private Label lblUserInfo = null!;

        private readonly List<Button> _navButtons = new();
        private string _currentModule = "dashboard";

        // --- عناصر وحدة Dashboard/Contrats ---
        private DataGridView dgvContracts = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbFilterStatus = null!;
        private ComboBox cmbFilterType = null!;

        // --- عناصر وحدة Avenants ---
        private DataGridView dgvAvenants = null!;

        // تعريف الوحدات: (المفتاح، الأيقونة، الاسم المعروض)
        private readonly (string Key, string Icon, string Label)[] _modules = new[]
        {
            ("dashboard",      "🏠", "Dashboard"),
            ("contrats",       "📄", "Contrats"),
            ("avenants",       "📑", "Avenants"),
            ("courriers",      "📬", "Courriers"),
            ("documents",      "📎", "Documents"),
            ("utilisateurs",   "👥", "Utilisateurs"),
            ("notifications",  "🔔", "Notifications"),
            ("rapports",       "📊", "Rapports"),
            ("parametres",     "⚙", "Paramètres"),
        };

        // وصف الوظائف المخطط لها لكل وحدة لم تُبنَ بعد (لعرضها في بطاقة "قيد الإنجاز")
        private readonly Dictionary<string, string[]> _plannedFeatures = new()
        {
            ["courriers"] = new[] { "Courriers entrants / sortants liés aux contrats", "Numérotation et suivi des références", "Rappels de relance automatiques" },
            ["documents"] = new[] { "Coffre-fort documentaire par contrat/partenaire", "Import PDF, Word, images", "Recherche par mots-clés et tags" },
            ["notifications"] = new[] { "Alertes d'échéance de contrats", "Rappels de paiement / facturation", "Centre de notifications en temps réel" },
            ["rapports"] = new[] { "Rapports financiers par période", "Export Excel / PDF personnalisé", "Statistiques par type de contrat / partenaire" },
            ["parametres"] = new[] { "Gestion des rôles et permissions", "Paramètres généraux (devise, numérotation)", "Sauvegarde et restauration de la base" },
        };

        public FrmMain() : this("Administrateur", "ADMIN") { }

        public FrmMain(string userDisplay, string userCode)
        {
            _userDisplay = userDisplay;
            _userCode = userCode;
            InitializeComponent();
            AppAssets.ApplyIcon(this);
            BuildShell();
            SwitchModule("dashboard");
        }

        // ============================================================
        // الهيكل العام: Sidebar + TopBar + Content
        // ============================================================
        private void BuildShell()
        {
            this.BackColor = Theme.PageBg;
            this.Font = new Font("Segoe UI", 9F);
            this.MinimumSize = new Size(1050, 650);
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlContent = new Panel { Dock = DockStyle.Fill, BackColor = Theme.PageBg };
            pnlTopBar = BuildTopBar();
            pnlSidebar = BuildSidebar();

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTopBar);
            this.Controls.Add(pnlSidebar);
        }

        // ============================================================
        // SIDEBAR (أخضر جزائري غامق) — قائمة الوحدات
        // ============================================================
        private Panel BuildSidebar()
        {
            Panel pnl = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = Theme.SidebarBg };

            Panel pnlLogo = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Theme.SidebarBg };
            Control logoControl;
            if (AppAssets.AppIcon != null)
            {
                logoControl = new PictureBox
                {
                    Image = AppAssets.AppIcon.ToBitmap(),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(38, 38),
                    Location = new Point(16, 16),
                    BackColor = Theme.SidebarBg
                };
            }
            else
            {
                logoControl = new LogoBadge { Size = new Size(36, 36), Letter = "N", Location = new Point(18, 17), BackColor = Theme.SidebarBg };
            }
            Label lblLogo = new Label
            {
                Text = "NovaGestion",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(62, 25),
                AutoSize = true
            };
            pnlLogo.Controls.Add(logoControl);
            pnlLogo.Controls.Add(lblLogo);

            pnlNav = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Theme.SidebarBg,
                Padding = new Padding(0, 10, 0, 0)
            };

            foreach (var m in _modules)
            {
                Button btn = new Button
                {
                    Text = $"   {m.Icon}    {m.Label}",
                    Width = 230,
                    Height = 48,
                    TextAlign = ContentAlignment.MiddleLeft,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Theme.SidebarBg,
                    ForeColor = Color.White,
                    Font = Theme.FontValue,
                    Cursor = Cursors.Hand,
                    Tag = m.Key,
                    Margin = new Padding(0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Theme.SidebarHover;
                btn.Click += (s, e) => SwitchModule((string)((Button)s!).Tag);
                _navButtons.Add(btn);
                pnlNav.Controls.Add(btn);
            }

            Label lblVersion = new Label
            {
                Text = "NovaGestion v1.0",
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(200, 220, 210),
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnl.Controls.Add(pnlNav);
            pnl.Controls.Add(lblVersion);
            pnl.Controls.Add(pnlLogo);
            return pnl;
        }

        private void HighlightActiveNav(string key)
        {
            foreach (Button b in _navButtons)
            {
                bool active = (string)b.Tag == key;
                b.BackColor = active ? Theme.SidebarActive : Theme.SidebarBg;
                b.Font = active ? Theme.FontBold : Theme.FontValue;
            }
        }

        // ============================================================
        // TOP BAR (أبيض) — عنوان الوحدة الحالية + معلومات المستخدم
        // ============================================================
        private Panel BuildTopBar()
        {
            Panel pnl = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Color.White };
            Panel bottomLine = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Theme.BorderGray };

            lblPageIcon = new Label
            {
                Text = "🏠",
                Font = new Font("Segoe UI", 15F),
                Location = new Point(24, 16),
                AutoSize = true
            };
            lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Theme.TextDark,
                Location = new Point(58, 17),
                AutoSize = true
            };

            lblUserInfo = new Label
            {
                Text = $"{_userDisplay} [{_userCode}]",
                Font = Theme.FontValue,
                ForeColor = Theme.TextGray,
                AutoSize = true
            };

            RoundedButton btnLogout = new RoundedButton
            {
                Text = "⏻ Déconnexion",
                Size = new Size(140, 38),
                Font = Theme.FontButton,
                BackColor = Theme.MaroonAccent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) => Logout();

            void Reposition()
            {
                btnLogout.Location = new Point(pnl.Width - btnLogout.Width - 20, 13);
                lblUserInfo.Location = new Point(btnLogout.Left - lblUserInfo.Width - 20, 23);
            }
            pnl.Resize += (s, e) => Reposition();
            pnl.HandleCreated += (s, e) => Reposition();

            pnl.Controls.AddRange(new Control[] { lblPageIcon, lblPageTitle, lblUserInfo, btnLogout, bottomLine });
            return pnl;
        }

        // ============================================================
        // التنقل بين الوحدات
        // ============================================================
        private void SwitchModule(string key)
        {
            if (key == "utilisateurs")
            {
                using FrmUsers f = new FrmUsers();
                f.ShowDialog(this);
                return;
            }

            _currentModule = key;
            HighlightActiveNav(key);

            var m = Array.Find(_modules, x => x.Key == key);
            lblPageIcon.Text = m.Icon;
            lblPageTitle.Text = m.Label;
            this.Text = $"NovaGestion - {m.Label}";

            pnlContent.Controls.Clear();
            Panel content = key switch
            {
                "dashboard" => BuildDashboardContent(),
                "contrats" => BuildContratsContent(),
                "avenants" => BuildAvenantsContent(),
                _ => BuildPlaceholderContent(m.Icon, m.Label)
            };
            content.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(content);
        }

        // ============================================================
        // DASHBOARD: Résumé + Actions rapides + جدول العقود
        // ============================================================
        private Panel BuildDashboardContent()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Theme.PageBg };

            Panel pnlLeft = new Panel { Dock = DockStyle.Left, Width = 280, BackColor = Theme.PageBg };
            Panel pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15, 0, 0, 0), BackColor = Theme.PageBg };

            Panel spacer = new Panel { Dock = DockStyle.Top, Height = 15, BackColor = Theme.PageBg };
            pnlLeft.Controls.Add(BuildActionsCard());
            pnlLeft.Controls.Add(spacer);
            pnlLeft.Controls.Add(BuildResumeCard());

            pnlRight.Controls.Add(BuildGridArea());
            pnlRight.Controls.Add(BuildToolbar());

            root.Controls.Add(pnlRight);
            root.Controls.Add(pnlLeft);
            return root;
        }

        private RoundedPanel BuildResumeCard()
        {
            RoundedPanel card = new RoundedPanel { Dock = DockStyle.Top, Size = new Size(280, 210) };

            Label lblTitle = new Label { Text = "Résumé", Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(16, 14), AutoSize = true };
            card.Controls.Add(lblTitle);

            int totalContrats = AppData.Contrats.Count;
            decimal engagement = 0, facture = 0;
            foreach (var c in AppData.Contrats)
            {
                engagement += c.MontantTTC;
                facture += c.FactureDA;
            }
            decimal reste = engagement - facture;

            AddStatBlock(card, "CONTRATS", totalContrats.ToString(), Theme.Navy, 16, 50);
            AddStatBlock(card, "ENGAGEMENT", engagement.ToString("N0") + " DA", Theme.Navy, 145, 50);
            AddStatBlock(card, "FACTURÉ", facture.ToString("N0") + " DA", Theme.Green, 16, 115);
            AddStatBlock(card, "PAYÉ", facture.ToString("N0") + " DA", Theme.Green, 145, 115);
            AddStatBlock(card, "DETTE", "0 DA", Theme.Red, 16, 165);
            AddStatBlock(card, "RESTE", reste.ToString("N0") + " DA", Theme.Orange, 145, 165);

            return card;
        }

        private void AddStatBlock(Control parent, string title, string value, Color color, int x, int y)
        {
            Label lblTitle = new Label { Text = title, Font = new Font("Segoe UI", 7.5F, FontStyle.Bold), ForeColor = Theme.TextMuted, Location = new Point(x, y), AutoSize = true };
            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = color, Location = new Point(x, y + 16), AutoSize = true };

            // تم تغيير لون الخلفية للخط إلى شفاف
            Panel underline = new Panel { BackColor = Color.Transparent, Size = new Size(110, 2), Location = new Point(x, y + 44) };

            parent.Controls.Add(lblTitle);
            parent.Controls.Add(lblValue);
            parent.Controls.Add(underline);
        }

        private RoundedPanel BuildActionsCard()
        {
            RoundedPanel card = new RoundedPanel { Dock = DockStyle.Top, Size = new Size(280, 285) };

            Label lblTitle = new Label { Text = "Actions rapides", Font = Theme.FontSectionTitle, ForeColor = Theme.TextDark, Location = new Point(16, 14), AutoSize = true };
            card.Controls.Add(lblTitle);

            RoundedButton btnNewContract = MakeActionButton("+ Nouveau Contrat", Theme.Maroon, Color.White, 50);
            btnNewContract.FlatAppearance.MouseOverBackColor = Theme.MaroonAccent;
            btnNewContract.Click += (s, e) => OpenNewContract();

            RoundedButton btnExcel = MakeActionButton("📊 Export Excel", Theme.BtnLightBg, Theme.TextDark, 96);
            btnExcel.Click += (s, e) => MessageBox.Show("Export Excel — à implémenter.", "Info");

            RoundedButton btnPdf = MakeActionButton("📄 Export PDF", Theme.BtnLightBg, Theme.TextDark, 142);
            btnPdf.Click += (s, e) => MessageBox.Show("Export PDF — à implémenter.", "Info");

            RoundedButton btnBackup = MakeActionButton("💾 Sauvegarde", Theme.BtnLightBg, Theme.TextDark, 188);
            btnBackup.Click += (s, e) => MessageBox.Show("Sauvegarde — à implémenter.", "Info");

            RoundedButton btnUsers = MakeActionButton("👥 Utilisateurs", Theme.BtnLightBg, Theme.TextDark, 234);
            btnUsers.Click += (s, e) =>
            {
                using FrmUsers f = new FrmUsers();
                f.ShowDialog(this);
            };

            card.Controls.AddRange(new Control[] { btnNewContract, btnExcel, btnPdf, btnBackup, btnUsers });
            return card;
        }

        private RoundedButton MakeActionButton(string text, Color bg, Color fg, int y)
        {
            var btn = new RoundedButton
            {
                Text = text,
                Location = new Point(16, y),
                Size = new Size(248, 38),
                Font = Theme.FontButton,
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = bg;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 235, 240);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 218, 226);

            return btn;
        }

        // ============================================================
        // وحدة CONTRATS (نفس الجدول لكن بعرض كامل + شريط بحث)
        // ============================================================
        private Panel BuildContratsContent()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Theme.PageBg };
            root.Controls.Add(BuildGridArea());
            root.Controls.Add(BuildToolbar());
            return root;
        }

        private Panel BuildToolbar()
        {
            Panel pnl = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Theme.PageBg };

            txtSearch = new TextBox
            {
                PlaceholderText = "🔍  Rechercher...",
                Location = new Point(0, 0),
                Size = new Size(400, 34),
                Font = Theme.FontValue,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => ApplyContractsFilter();

            cmbFilterStatus = new ComboBox { Location = new Point(410, 0), Size = new Size(150, 34), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterStatus.Items.AddRange(new object[] { "Tous statuts", "ACTIF", "INACTIF", "RÉSILIÉ" });
            cmbFilterStatus.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndexChanged += (s, e) => ApplyContractsFilter();

            cmbFilterType = new ComboBox { Location = new Point(570, 0), Size = new Size(150, 34), Font = Theme.FontValue, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFilterType.Items.AddRange(new object[] { "Tous types", "STL", "Prestation", "Location" });
            cmbFilterType.SelectedIndex = 0;
            cmbFilterType.SelectedIndexChanged += (s, e) => ApplyContractsFilter();

            RoundedButton btnNew = new RoundedButton
            {
                Text = "+ Nouveau Contrat",
                Size = new Size(170, 34),
                Font = Theme.FontButton,
                BackColor = Theme.Maroon,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.FlatAppearance.MouseOverBackColor = Theme.MaroonAccent;
            btnNew.Click += (s, e) => OpenNewContract();

            Label lblCount = new Label
            {
                Text = $"📋  Contrats ({AppData.Contrats.Count})",
                Font = Theme.FontSectionTitle,
                ForeColor = Theme.TextDark,
                Location = new Point(0, 50),
                AutoSize = true
            };

            pnl.Controls.AddRange(new Control[] { txtSearch, cmbFilterStatus, cmbFilterType, btnNew, lblCount });

            void Reposition() => btnNew.Location = new Point(Math.Max(0, pnl.Width - btnNew.Width), 0);
            pnl.Resize += (s, e) => Reposition();
            Reposition();

            return pnl;
        }

        private Panel BuildGridArea()
        {
            Panel container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };

            dgvContracts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(235, 238, 242),
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = Theme.FontValue
            };

            dgvContracts.ColumnHeadersDefaultCellStyle.BackColor = Theme.Maroon;
            dgvContracts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvContracts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvContracts.ColumnHeadersHeight = 40;
            dgvContracts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230);
            dgvContracts.DefaultCellStyle.SelectionForeColor = Theme.Maroon;

            dgvContracts.Columns.Add("NumContrat", "N° Contrat");
            dgvContracts.Columns.Add("RaisonSociale", "Raison Sociale");
            dgvContracts.Columns.Add("Type", "Type");
            dgvContracts.Columns.Add("Statut", "Statut");
            dgvContracts.Columns.Add("MontantDA", "Montant DA");
            dgvContracts.Columns.Add("FactureDA", "Facturé DA");
            dgvContracts.Columns.Add("Pourcentage", "%");

            ApplyContractsFilter();

            container.Controls.Add(dgvContracts);
            return container;
        }

        private void ApplyContractsFilter()
        {
            if (dgvContracts == null) return;

            string searchText = txtSearch?.Text.Trim().ToLower() ?? "";
            string selectedStatus = cmbFilterStatus?.SelectedItem?.ToString() ?? "Tous statuts";
            string selectedType = cmbFilterType?.SelectedItem?.ToString() ?? "Tous types";

            dgvContracts.Rows.Clear();

            foreach (var c in AppData.Contrats)
            {
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                                     c.NumContrat.ToLower().Contains(searchText) ||
                                     c.RaisonSociale.ToLower().Contains(searchText);

                bool matchesStatus = selectedStatus == "Tous statuts" ||
                                     c.Statut.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase);

                bool matchesType = selectedType == "Tous types" ||
                                   c.Type.Equals(selectedType, StringComparison.OrdinalIgnoreCase);

                if (matchesSearch && matchesStatus && matchesType)
                {
                    string pct = c.MontantTTC > 0 ? $"{(c.FactureDA / c.MontantTTC * 100):0}%" : "0%";
                    dgvContracts.Rows.Add(c.NumContrat, c.RaisonSociale, c.Type, c.Statut,
                        c.MontantTTC.ToString("N0"), c.FactureDA.ToString("N0"), pct);
                }
            }
        }

        // ============================================================
        // بطاقة "قيد الإنجاز" للوحدات غير المبنية بعد (تعرض الخطة المستقبلية)
        // ============================================================
        private Panel BuildPlaceholderContent(string icon, string label)
        {
            Panel root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Theme.PageBg };

            RoundedPanel card = new RoundedPanel { Dock = DockStyle.Top, Size = new Size(600, 320) };

            Label lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 32F), Location = new Point(30, 25), AutoSize = true };
            Label lblTitle = new Label { Text = label, Font = Theme.FontTitle, ForeColor = Theme.TextDark, Location = new Point(95, 35), AutoSize = true };
            Label lblStatus = new Label
            {
                Text = "🚧  Module en cours de développement",
                Font = Theme.FontBold,
                ForeColor = Theme.MaroonAccent,
                Location = new Point(30, 90),
                AutoSize = true
            };

            Label lblFeatTitle = new Label
            {
                Text = "Fonctionnalités prévues :",
                Font = Theme.FontBold,
                ForeColor = Theme.TextDark,
                Location = new Point(30, 130),
                AutoSize = true
            };

            card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblStatus, lblFeatTitle });

            int fy = 160;
            if (_plannedFeatures.TryGetValue(_currentModule, out string[]? feats))
            {
                foreach (string feat in feats)
                {
                    Label lbl = new Label
                    {
                        Text = "•  " + feat,
                        Font = Theme.FontValue,
                        ForeColor = Theme.TextGray,
                        Location = new Point(30, fy),
                        AutoSize = true
                    };
                    card.Controls.Add(lbl);
                    fy += 30;
                }
            }

            root.Controls.Add(card);
            return root;
        }

        // ============================================================
        // وحدة AVENANTS
        // ============================================================
        private Panel BuildAvenantsContent()
        {
            Panel root = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = Theme.PageBg };
            root.Controls.Add(BuildAvenantsGridArea());
            root.Controls.Add(BuildAvenantsToolbar());
            return root;
        }

        private Panel BuildAvenantsToolbar()
        {
            Panel pnl = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Theme.PageBg };

            TextBox txtSearchAv = new TextBox
            {
                PlaceholderText = "🔍  Rechercher un avenant...",
                Location = new Point(0, 0),
                Size = new Size(400, 34),
                Font = Theme.FontValue,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchAv.TextChanged += (s, e) => ApplyAvenantsFilter(txtSearchAv.Text);

            RoundedButton btnNew = new RoundedButton
            {
                Text = "+ Nouvel Avenant",
                Size = new Size(170, 34),
                Font = Theme.FontButton,
                BackColor = Theme.Maroon,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.FlatAppearance.MouseOverBackColor = Theme.MaroonAccent;
            btnNew.Click += (s, e) => OpenNewAvenant();

            Label lblCount = new Label
            {
                Text = $"📑  Avenants ({AppData.Avenants.Count})",
                Font = Theme.FontSectionTitle,
                ForeColor = Theme.TextDark,
                Location = new Point(0, 50),
                AutoSize = true
            };

            pnl.Controls.AddRange(new Control[] { txtSearchAv, btnNew, lblCount });

            void Reposition() => btnNew.Location = new Point(Math.Max(0, pnl.Width - btnNew.Width), 0);
            pnl.Resize += (s, e) => Reposition();
            Reposition();

            return pnl;
        }

        private Panel BuildAvenantsGridArea()
        {
            Panel container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };

            dgvAvenants = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(235, 238, 242),
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 },
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = Theme.FontValue
            };

            dgvAvenants.ColumnHeadersDefaultCellStyle.BackColor = Theme.Maroon;
            dgvAvenants.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAvenants.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvAvenants.ColumnHeadersHeight = 40;
            dgvAvenants.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 245, 230);
            dgvAvenants.DefaultCellStyle.SelectionForeColor = Theme.Maroon;

            dgvAvenants.Columns.Add("NumAvenant", "N° Avenant");
            dgvAvenants.Columns.Add("NumContrat", "N° Contrat");
            dgvAvenants.Columns.Add("RaisonSociale", "Raison Sociale");
            dgvAvenants.Columns.Add("TypeModif", "Type Modification");
            dgvAvenants.Columns.Add("Date", "Date");
            dgvAvenants.Columns.Add("Statut", "Statut");

            ApplyAvenantsFilter("");

            container.Controls.Add(dgvAvenants);
            return container;
        }

        private void ApplyAvenantsFilter(string query)
        {
            if (dgvAvenants == null) return;

            string searchText = query.Trim().ToLower();
            dgvAvenants.Rows.Clear();

            foreach (var a in AppData.Avenants)
            {
                bool matches = string.IsNullOrEmpty(searchText) ||
                               a.NumAvenant.ToLower().Contains(searchText) ||
                               a.NumContratLie.ToLower().Contains(searchText) ||
                               a.RaisonSociale.ToLower().Contains(searchText);

                if (matches)
                {
                    dgvAvenants.Rows.Add(a.NumAvenant, a.NumContratLie, a.RaisonSociale, a.TypeModification,
                        a.DateAvenant.ToShortDateString(), a.Statut);
                }
            }
        }

        private void OpenNewAvenant()
        {
            if (AppData.Contrats.Count == 0)
            {
                MessageBox.Show("Veuillez d'abord créer un contrat avant d'ajouter un avenant.",
                    "Aucun contrat disponible", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using FrmAvenant f = new FrmAvenant();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                SwitchModule(_currentModule);
            }
        }

        private void OpenNewContract()
        {
            using FrmContracts f = new FrmContracts();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                SwitchModule(_currentModule);
            }
        }

        private void Logout()
        {
            DialogResult r = MessageBox.Show("Voulez-vous vraiment vous déconnecter ?", "Déconnexion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}