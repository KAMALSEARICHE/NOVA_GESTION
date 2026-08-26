namespace NovaGestion
{
    partial class FrmUsers
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// كامل عناصر هذه الشاشة تُبنى برمجياً داخل FrmUsers.cs (طريقة BuildUI)
        /// لتبقى منسجمة مع باقي شاشات NovaGestion، وقابلة للسكرة بصفة عادية
        /// (خلافاً للنسخة القديمة اللي كانت FormBorderStyle=None بلا زر إغلاق).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 650);
            this.Name = "FrmUsers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "NovaGestion - Utilisateurs & Partenaires";
        }

        #endregion
    }
}
